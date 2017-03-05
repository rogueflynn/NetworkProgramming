import Queue

q = Queue.Queue()

q.put('bob')
q.put('sam')
q.put('twich')

print q.get_nowait()


