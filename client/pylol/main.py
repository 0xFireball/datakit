import socket

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

sock.connect(('127.0.0.1', 5503))


def getpacket(transport):
    n = False
    data = ""
    while not n:
        r = transport.recv(1).decode()
        if r != '\n':
            data += r
        else:
            n = True
    return data


# send hello
result = sock.sendall('$H|lolpy fake sensor|stream\n'.encode())
ack = getpacket(sock)
print(ack)

