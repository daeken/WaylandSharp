import sys
from glob import glob
from xml.dom.minidom import parse

def getText(pnode):
	return ''.join(node.data for node in pnode.childNodes if node.nodeType == node.TEXT_NODE)

def getDesc(node):
	summary = node.getAttribute('summary')
	for child in node.childNodes:
		if child.nodeType == node.ELEMENT_NODE and child.tagName == 'description':
			nsum = child.getAttribute('summary')
			return summary or nsum or None, getText(child)
	return summary or None, None

def atName(name):
	if name in ('interface', ):
		return '@' + name
	return name

def rename(name):
	name = ''.join(elem.title() for elem in name.split('_'))
	if name[0] in '0123456789':
		name = '_' + name
	return atName(name)

def fupper(str):
	return str[0].upper() + str[1:]

def doc(indent, elem):
	summary, desc = getDesc(elem)
	if summary is not None:
		print '%s/// <summary>' % indent
		print '%s///' % indent, fupper(summary).encode('utf-8')
		print '%s/// </summary>' % indent
	if desc is not None:
		print '%s/// <remarks>' % indent
		for line in desc.strip().split('\n'):
			print '%s///' % indent, line.strip().encode('utf-8')
		print '%s/// </remarks>' % indent
	for arg in elem.getElementsByTagName('arg'):
		if arg.parentNode != elem:
			continue
		name = arg.getAttribute('name')
		summary = arg.getAttribute('summary')
		if summary:
			print '\t\t/// <param name="%s">%s</param>' % (name, fupper(summary))

def generate(fn):
	dom = parse(file(fn, 'r'))
	for interface in dom.getElementsByTagName('interface'):
		doc('\t', interface)
		iname = 'I' + rename(interface.getAttribute('name'))
		print '\tpublic abstract class %s : IWaylandObject {' % iname
		print '\t\tpublic override string InterfaceName => "%s";' % interface.getAttribute('name')
		print '\t\tpublic override int InterfaceVersion => %s;' % interface.getAttribute('version')
		print '\t\tprotected %s(Client owner, uint? id) : base(owner, id) {}' % iname
		if len(interface.getElementsByTagName('enum')):
			print '\t\tpublic static class Enum {'
			for enum in interface.getElementsByTagName('enum'):
				doc('\t\t\t', enum)
				bc = ''
				if enum.getAttribute('bitfield') == 'true':
					print '\t\t\t[Flags]'
					bc = ' : uint'
				print '\t\t\tpublic enum %s%s {' % (rename(enum.getAttribute('name')), bc)
				for elem in enum.getElementsByTagName('entry'):
					doc('\t\t\t\t', elem)
					print '\t\t\t\t%s = %s,' % (rename(elem.getAttribute('name')), elem.getAttribute('value'))
				print '\t\t\t}'
			print '\t\t}'
		print '\t\tinternal override void ProcessRequest(int opcode, byte[] tbuf) {'
		requests = []
		if len(interface.getElementsByTagName('request')) == 0:
			print '\t\t\tthrow new NotSupportedException();'
		else:
			print '\t\t\tvar _offset = 0;'
			print '\t\t\tswitch(opcode) {'
			for opcode, request in enumerate(interface.getElementsByTagName('request')):
				rname = rename(request.getAttribute('name'))
				args = []
				cargs = []
				hasFd = False
				outObjs = []
				ret_type = 'void'
				ret_keep = None
				outs = sum([1 for arg in request.getElementsByTagName('arg') if arg.getAttribute('type') == 'new_id'])
				print '\t\t\t\tcase %i: {' % opcode
				for arg in request.getElementsByTagName('arg'):
					name = atName(arg.getAttribute('name'))
					type = arg.getAttribute('type')
					if type == 'object' or type == 'new_id':
						iface = arg.getAttribute('interface')
						if iface:
							ctype = 'I' + rename(iface)
						else:
							ctype = 'IWaylandObject'
						if type == 'object':
							print '\t\t\t\t\tvar %s = Owner.GetObject<%s>(Helper.ReadUint(tbuf, ref _offset));' % (name, ctype)
							cargs.append(name)
						else:
							if ctype == 'IWaylandObject':
								#ctype = 'NewIdUnknown'
								print '\t\t\t\t\tvar %s_iname = Helper.ReadString(tbuf, ref _offset);' % name
								print '\t\t\t\t\tvar %s_version = Helper.ReadUint(tbuf, ref _offset);' % name
								print '\t\t\t\t\tvar %s_newid = Helper.ReadUint(tbuf, ref _offset);' % name
								#cargs.append('new NewIdUnknown(%s_iname, %s_version, %s_newid)' % (name, name, name))
							else:
								print '\t\t\t\t\tvar %s_newid = Helper.ReadUint(tbuf, ref _offset);' % name
							if outs == 1:
								ret_type = ctype
								ret_keep = name
							else:
								cargs.append('out var %s' % name)
							outObjs.append('Owner.SetObject(%s_newid, %s);' % (name, name))
					elif type == 'int' or type == 'uint':
						ctype = type
						enum = arg.getAttribute('enum')
						if enum:
							enum = rename(enum)
							if '.' in enum:
								cname, ename = enum.rsplit('.', 1)
								ctype = 'I%s.Enum.%s' % (cname, ename)
							else:
								ctype = 'Enum.%s' % enum
						print '\t\t\t\t\tvar %s = (%s) Helper.Read%s(tbuf, ref _offset);' % (name, ctype, 'Uint' if type == 'uint' else 'Int')
						cargs.append(name)
					elif type == 'fixed':
						ctype = 'Fixed'
						print '\t\t\t\t\tvar %s = new Fixed(Helper.ReadUint(tbuf, ref _offset));' % name
						cargs.append(name)
					elif type == 'array':
						ctype = 'byte[]'
						print '\t\t\t\t\tvar %s = Helper.ReadArray(tbuf, ref _offset);' % name
						cargs.append(name)
					elif type == 'fd':
						ctype = 'int'
						cargs.append('Owner.GetNextFd()')
					elif type == 'string':
						ctype = 'string'
						print '\t\t\t\t\tvar %s = Helper.ReadString(tbuf, ref _offset);' % name
						cargs.append(name)
					else:
						print type
						assert False
					if type == 'new_id' and outs == 1:
						pass
					else:
						args.append('%s%s %s' % ('out ' if type == 'new_id' else '', ctype, name))
				if ret_keep is None:
					print '\t\t\t\t\t%s(%s);' % (rname, ', '.join(cargs))
				else:
					print '\t\t\t\t\tvar %s = %s(%s);' % (ret_keep, rname, ', '.join(cargs))
				for line in outObjs:
					print '\t\t\t\t\t' + line
				print '\t\t\t\t\tbreak;'
				print '\t\t\t\t}'
				requests.append((request, '\t\tpublic abstract %s %s(%s);' % (ret_type, rname, ', '.join(args))))
			print '\t\t\t}'
		print '\t\t}'
		for request, line in requests:
			doc('\t\t', request)
			print line
		for opcode, event in enumerate(interface.getElementsByTagName('event')):
			doc('\t\t', event)
			args = []
			hasFd = False
			pre = ['var _offset = 0;']
			post = []
			for arg in event.getElementsByTagName('arg'):
				name = atName(arg.getAttribute('name'))
				type = arg.getAttribute('type')
				if type == 'object' or type == 'new_id':
					iface = arg.getAttribute('interface')
					if iface:
						ctype = 'I' + rename(iface)
					else:
						ctype = 'IWaylandObject'
					pre.append('_offset += 4;')
					post.append('Helper.WriteUint(tbuf, ref _offset, %s.Id);' % name)
				elif type == 'int' or type == 'uint':
					ctype = type
					enum = arg.getAttribute('enum')
					if enum:
						enum = rename(enum)
						if '.' in enum:
							cname, ename = enum.rsplit('.', 1)
							ctype = 'I%s.Enum.%s' % (cname, ename)
						else:
							ctype = 'Enum.%s' % enum
					pre.append('_offset += 4;')
					post.append('Helper.Write%s(tbuf, ref _offset, (%s) %s);' % ('Uint' if type == 'uint' else 'Int', type, name))
				elif type == 'fixed':
					ctype = 'Fixed'
					pre.append('_offset += 4;')
					post.append('Helper.WriteUint(tbuf, ref _offset, %s.UintValue);' % name)
				elif type == 'array':
					ctype = 'byte[]'
					pre.append('_offset += Helper.ArraySize(%s);' % name)
					post.append('Helper.WriteArray(tbuf, ref _offset, %s);' % name)
				elif type == 'fd':
					ctype = 'int'
					assert not hasFd
					hasFd = True
					pre.append('var _fd = (int) %s;' % name)
				elif type == 'string':
					ctype = 'string'
					pre.append('_offset += Helper.StringSize(%s);' % name)
					post.append('Helper.WriteString(tbuf, ref _offset, %s);' % name)
				else:
					print type
					assert False
				args.append('%s %s' % (ctype, name))
			print '\t\tpublic void %s(%s) {' % (rename(event.getAttribute('name')), ', '.join(args))
			for line in pre:
				print '\t\t\t' + line
			print '\t\t\tvar tbuf = new byte[_offset];'
			print '\t\t\t_offset = 0;'
			for line in post:
				print '\t\t\t' + line
			if hasFd:
				print '\t\t\tSendEventWithFd(%i, tbuf, _fd);' % opcode
			else:
				print '\t\t\tSendEvent(%i, tbuf);' % opcode
			print '\t\t}'
		print '\t}'

def main():
	print '#pragma warning disable 0219'
	print '#pragma warning disable 1998'
	print '#pragma warning disable 8321'
	print 'using System;'
	print 'using System.Threading.Tasks;'
	print 'namespace WaylandSharp.Generated {'
	for fn in glob('protocol_xml/*.xml'):
		generate(fn)
	print '}'

if __name__=='__main__':
	main(*sys.argv[1:])
