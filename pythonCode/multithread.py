import threading
import time

exitFlag = 0

class clientThread (threading.Thread):
	def __init__(self, threadID, name, counter):
		threading.Thread.__init__(self)
		self.threadID = threadID
		self.name = name
		self.counter = counter
	def run(self):
		print ("Starting " + self.name)
		if self.name == "hello":
			print_hello(self.name, self.counter, 5)
		else:
			print_time(self.name, self.counter, 5)
		print("Exiting " + self.name)


def print_time(threadName, delay, counter):
	while counter:
		time.sleep(delay)
		print ("%s: %s" % (threadName, time.ctime(time.time())))
		counter -= 1
def print_hello(threadName, delay, counter):
	while counter:
		time.sleep(delay)
		print ("Hello")
		counter -= 1

#create new threads
thread1 = clientThread(1, "Thread-1", 1)	
thread2 = clientThread(2, "Thread-2", 2)	
thread3 = clientThread(3, "hello", 3)	

#start threads
thread1.start()
thread2.start()
thread3.start()

#wait for thread to terminate
thread1.join()
thread2.join()
thread3.join()

print("Exiting main thread")
