import socket
import threading
import time

server_data = ('127.0.0.1', 5503)


class Sensor(object):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect(server_data)
    leader = None
    follower = None
    heartbeater = None
    go = False
    run = True

    def __init__(self, name, units, function):
        result = self.sock.sendall(('$H|%s|%s|replace|stream\n' % (name, units)).encode())
        ack = self.getpacket(self.sock)
        self.guid = ack.split("|")[1]

        # Starting heartbeating
        self.heartbeater = threading.Thread(target=self.heartbeat)
        self.heartbeater.start()
        print(self.guid)
        self.get_data = function

        # Starting leader (checks for STOP/START)
        self.leader = threading.Thread(target=self.start_stop, name="leader")
        self.leader.start()

        # Initializes follower (sends data to server)
        self.follower = threading.Thread(target=self.read_data, name="follower")

    # Checks for START/STOP from server
    def start_stop(self):
        while self.run:
            data = self.getpacket(self.sock).strip()
            if data == "START":
                go = True
                self.follower.start()
            if data == "STOP":
                go = False

    # Reads and sends data (TODO:SEND DATA)
    def read_data(self):
        self.go = True
        while self.go:
            self.sock.sendall(str(self.get_data()+'\n').endcode())
            time.sleep(.1)

    # Heartbeater
    def heartbeat(self):
        while True:
            self.sock.sendall('$P\n'.encode())
            time.sleep(1)

    # Helper function to get data from web sockets
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
