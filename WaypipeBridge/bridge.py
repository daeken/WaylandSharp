import os, os.path, struct, sys, tempfile
from socket import *
from select import select
from subprocess import Popen, PIPE, STDOUT

def run(sock, host):
	assert struct.unpack('I', sock.recv(4))[0] == 0xDEADBEEF
	tport, = struct.unpack('H', sock.recv(2))
	cmdlen, = struct.unpack('I', sock.recv(4))
	cmd = sock.recv(cmdlen)

	usock = socket(AF_UNIX, SOCK_STREAM)
	spath = os.path.join(tempfile.mkdtemp(), 'serversock')
	usock.bind(spath)
	usock.listen(100)
	wpi = Popen(['waypipe', '-s', spath, 'server', '--', cmd])
	bc = []
	bcm = {}
	while True:
		rlist, _, xlist = select([sock, usock] + bc, (), ())
		if not wpi.poll() is None:
			return
		if sock in rlist or sock in xlist: # Either it sent something we don't know, or it disconnected
			break
		if usock in rlist:
			nuc, _ = usock.accept()
			nuc.setblocking(0)
			ntc = socket(AF_INET, SOCK_STREAM)
			ntc.connect((host, tport))
			ntc.setblocking(0)
			bc.append(nuc)
			bc.append(ntc)
			bcm[nuc.fileno()] = ntc
			bcm[ntc.fileno()] = nuc
		for csock in xlist:
			if csock.fileno() in bcm:
				cp = bcm[csock.fileno()]
				del bcm[csock.fileno()]
				del bcm[cp.fileno()]
				bc.remove(csock)
				bc.remove(cp)
				try:
					cp.close()
				except:
					pass
				try:
					csock.close()
				except:
					pass
		for csock in rlist:
			if csock.fileno() in bcm:
				cp = bcm[csock.fileno()]
				while True:
					try:
						data = csock.recv(1024)
						if len(data) == 0:
							break
					except:
						break
					off = 0
					while off < len(data):
						try:
							off += cp.send(data[off:])
						except:
							pass

def main(port):
	server = socket(AF_INET, SOCK_STREAM)
	server.setsockopt(SOL_SOCKET, SO_REUSEADDR, 1)
	server.bind(('', int(port)))
	server.listen(10)

	while True:
		sock, info = server.accept()
		if os.fork() == 0:
			run(sock, info[0])
			return

if __name__=='__main__':
	main(*sys.argv[1:])
