import socket

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

sock.connect(('127.0.0.1', 5503))

# send hello
sock.sendall('$H|lolpy fake sensor|stream')