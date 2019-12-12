import os, os.path, struct, sys, tempfile
from socket import *
from select import select
from subprocess import Popen, PIPE

def run(sock, host):
	assert struct.unpack('I', sock.recv(4))[0] == 0xDEADBEEF
	tport, = struct.unpack('H', sock.recv(2))

	usock = socket(AF_UNIX, SOCK_STREAM)
	spath = os.path.join(tempfile.mkdtemp(), 'serversock')
	usock.bind(spath)
	usock.listen(100)
	wpi = Popen(['waypipe', 'server', '-S', spath, '--', 'weston-terminal'], stdout=PIPE, stderr=PIPE)
	bc = []
	bcm = {}
	while True:
		rlist, _, xlist = select([wpi.stdout, wpi.stderr, sock, usock] + bc, (), ())
		if not wpi.poll() is None:
			return
		if sock in rlist or sock in xlist: # Either it sent something we don't know, or it disconnected
			break
		if wpi.stdout in rlist:
			sock.send('\0' + wpi.stdout.read(1))
		if wpi.stderr in rlist:
			sock.send('\x01' + wpi.stderr.read(1))
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
					data = csock.recv(1024)
					if len(data) == 0:
						break
					assert cp.send(data) == len(data)

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
