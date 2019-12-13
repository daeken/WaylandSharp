import sys
from socket import *
from select import select
from struct import pack, unpack

def main(host, port):
	server = socket(AF_INET, SOCK_STREAM)
	server.bind(('', 0))
	server.listen(100)
	msock = socket(AF_INET, SOCK_STREAM)
	msock.connect((host, int(port)))
	msock.send(pack('IH', 0xDEADBEEF, server.getsockname()[1]))
	msock.setblocking(0)

	stdout = ''
	stderr = ''
	conns = []
	while True:
		rlist, _, _ = select([server, msock] + conns, (), ())
		if server in rlist:
			conn, _ = server.accept()
			conn.setblocking(0)
			conns.append(conn)
		if msock in rlist:
			while True:
				try:
					c = msock.recv(1)
					print `c`
				except:
					break
				if c is None or c == '':
					break
				msock.setblocking(1)
				if c == '\0':
					stdout += msock.recv(1)
					print 'stdout', `stdout`
					if stdout.endswith('\n'):
						print 'Stdout:', stdout.rstrip()
						stdout = ''
				elif c == '\x01':
					stderr += msock.recv(1)
					print 'stderr', `stderr`
					if stderr.endswith('\n'):
						print 'Stderr:', stderr.rstrip()
						stderr = ''
				msock.setblocking(0)
		for s in rlist:
			if s in conns:
				print s, `s.recv(1024)`

if __name__=='__main__':
	main(*sys.argv[1:])
