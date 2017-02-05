import socket
import threading

server_data = ('127.0.0.1', 5503)

class Sensor(object):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect(server_data)
    master = None
    slave = None


    def __init__(self, name, units, function):
        result = self.sock.sendall(('$H|%s|%s|replace|stream\n' % (name, units)).encode())
        ack = self.getpacket(self.sock)
        self.guid = ack.split("|")[1]
        print(self.guid)
        self.get_data = function
        master = threading.Thread(target=self.start_stop, args=(self.sock,), name="master")
        master.start()
        ## slave = treading.Thread(target=self.get_data)

    def start_stop(self, sock):
        data = self.getpacket(sock)
        if data == "START":
            print("starting")

    def getpacket(self, transport):
        n = False
        data = ""
        while not n:
            r = transport.recv(1).decode()
            if r != '\n':
                data += r
            else:
                n = True
        return data
