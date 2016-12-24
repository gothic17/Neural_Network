import numpy as np
import random
import sys, os.path, time, logging
import math
import pickle
# Watchdog - for checking files
from watchdog.observers import Observer
from watchdog.events import PatternMatchingEventHandler
# Keras - for neural network
from keras.models import Sequential
from keras.layers.core import Dense, Dropout, Activation
from keras.optimizers import RMSprop

if os.path.isfile("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\\model.p"):
    print("POBIERAM Z PLIKU...")
    model = pickle.load(open("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\\model.p", "rb"))
else:
    print("TWORZE NOWY MODEL")
    model = Sequential()
    #model.add(Dense(164, init='lecun_uniform', input_shape=(24,)))
    model.add(Dense(11, init='lecun_uniform', input_shape=(11,)))
    model.add(Activation('relu'))
    #model.add(Dropout(0.2)) #I'm not using dropout, but maybe you wanna give it a try?

    model.add(Dense(164, init='lecun_uniform'))
    model.add(Activation('relu'))

    model.add(Dense(150, init='lecun_uniform'))
    model.add(Activation('relu'))
    #model.add(Dropout(0.2))

    model.add(Dense(10, init='lecun_uniform'))
    model.add(Activation('linear'))  # linear output so we can have range of real-valued outputs

    rms = RMSprop()
    model.compile(loss='mse', optimizer=rms)

def getReward(spineInclination, distance, previousDistance, spineHeight):
    print("REWARD: height = " + str(spineHeight) + ", spine = " + str(spineInclination) + ", dist = " + str(distance) + ", prev_dist = " + str(previousDistance))
    if (float(spineInclination) > 0.0 and float(spineInclination) < 90.0) and (distance > previousDistance):
        if (float(spineHeight) > 0.8):
            return 10
        else:
            return 5
    elif distance < 0:
        return -5
    else:
        return -1


def refactorInput(input):
    # Measure strength of the input array
    strength = 0.0
    for i in range (0, 11):
        strength += math.pow(float(input[i]), 2.0)
    strength = math.sqrt(strength)
    for i in range (0, 11):
        input[i] = float(input[i]) / strength
    return input

def runNetwork(string, previousDistance, epsilon, gamma, previousState, previousQval, previousAction):
    mode = "run"
    while True:
        try:
            with open("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\\input.txt", "r") as file:
                input = file.readlines()
                input = [x.strip('\n') for x in input]
                break
        except IOError:
            pass
    data = Data(previousState=None, previousQval=None, previousAction=None,
                previousDistance=None, epsilon=epsilon)
    y = [[previousQval]]
    if input.__len__() == 11:
        if previousState == None or previousQval == None or previousAction == None:
            previousState = input
            previousQval = model.predict(np.reshape(input, (1, 11)), batch_size=1)
            previousAction = 0
        spineHeight = input[0]
        spineInclination = input[1]
        distance = input[10]
        #----------------------------
        if mode == "learn":
            print("LEARNING...")
            # Observe reward
            reward = getReward(spineInclination, distance, previousDistance, spineHeight)
            print("REWARD = " + str(reward))
            # Refactor input array
            input = refactorInput(input)
            # Get max_Q(S',a)
            newQ = model.predict(np.reshape(input, (1, 11)), batch_size=1)
            maxQ = np.max(newQ)
            y = np.zeros((1, 10))
            y[:] = previousQval[:] # previousQval are predictions (Q values) of network from previous run
            if reward == -1:  # non-terminal state
                update = (reward + (gamma * maxQ))
            else:  # terminal state
                update = reward
            #update = (reward + (gamma * maxQ))
            y[0][previousAction] = update  # target output
            print("OUTPUT = " + str(y[0]))
            #print("Game #: %s" % (i,))
            #print("!!!!!!!! = " + str(previousState))
            #print np.reshape(previousState, (1, 24))
            model.fit(np.reshape(previousState, (1, 11)), y, batch_size=1, nb_epoch=1, verbose=1)
            #pickle.dump(model, open("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\\model.p", "wb"))
            previousState = input
            #if reward != -1:
            #    status = 0
            #clear_output(wait=True)
            #- - - - - - - - - - - - - - - - -
            # We are in state S
            # Let's run our Q function on S to get Q values for all possible actions
            qval = model.predict(np.reshape(input, (1, 11)), batch_size=1) #?????
            if (random.random() < epsilon):  # choose random action
                action = np.random.randint(0, 10)
            else:  # choose best action from Q(s,a) values
                action = (np.argmax(qval))
            if epsilon > 0.1:
                epsilon -= 0.00005
            print("EPSILON = " + str(epsilon))
            # Take action, observe new state S'
            #new_state = makeMove(state, action)
        else:
            print("RUNNING...")
            input = refactorInput(input)
            qval = model.predict(np.reshape(input, (1, 11)), batch_size=1)
            y[:] = qval[:]
            print("OUTPUT = " + str(y[0]))
            action = (np.argmax(qval))  # take action with highest Q-value
            print("ACTION = " + str(action))
        #---------------------------------------
        data.previousState = input
        data.previousQval = qval
        data.previousAction = action
        data.previousDistance = distance
        data.epsilon = epsilon
    outputStr = ""
    for i in range(len(y[0])):
        #ran = -1 + (2*random.uniform(0, 1))
        #outputStr += str(ran) + '\n'
        outputStr += str(y[0][i]) + '\n'
    while True:
        try:
            with open("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\output.txt", 'w') as file:
                file.write(outputStr)
                file.close()
                break
        except IOError:
            pass
    #return Data(previousState=input, previousQval=qval, previousAction=action, previousDistance=previousDistance)
    """print("Previous state = " + str(data.previousState) + " / previousQval = " + str(data.previousQval)
          + " / previousAction = " + str(data.previousAction) + " / previousDistance = "
          + str(data.previousDistance) + " / epsilon = " + str(data.epsilon))"""
    #return Data(previousState=input, previousQval=qval, previousAction=action,
    #            previousDistance=previousDistance, epsilon=epsilon)
    return data

class Data():
    def __init__(self, previousState, previousQval, previousAction, previousDistance, epsilon):
        self.previousState = previousState
        self.previousQval = previousQval
        self.previousAction = previousAction
        self.previousDistance = previousDistance
        self.epsilon = epsilon
        #print("DATA: prevState = " + str(previousState) + " / " + str(self.previousState) + " KONIEC!!!")

class MyEventHandler(PatternMatchingEventHandler):
    previousDistance = 0.0
    data = Data(previousState=None, previousQval=None, previousAction=None, previousDistance=previousDistance, epsilon=1.0)

    def on_moved(self, event):
        super(MyEventHandler, self).on_moved(event)
        logging.info("File %s was just moved" % event.src_path)

    def on_created(self, event):
        super(MyEventHandler, self).on_created(event)
        logging.info("File %s was just created" % event.src_path)

    def on_deleted(self, event):
        super(MyEventHandler, self).on_deleted(event)
        logging.info("File %s was just deleted" % event.src_path)

    def on_modified(self, event):
        super(MyEventHandler, self).on_modified(event)
        logging.info("File %s was just modified" % event.src_path)
        self.data = runNetwork("File %s was just modified" % event.src_path, previousDistance=self.previousDistance,
                               epsilon=self.data.epsilon, gamma=0.7, previousState=self.data.previousState,
                               previousQval=self.data.previousQval, previousAction=self.data.previousAction)
        #print(">>OUT<< prevState = " + str(self.data.previousState))
        self.previousDistance = self.data.previousDistance

def main(file_path=None):
    logging.basicConfig(level=logging.INFO,
        format='%(asctime)s - %(message)s',
        datefmt='%Y-%m-%d %H:%M:%S')
    watched_dir = os.path.split(file_path)[0]
    print 'watched_dir = {watched_dir}'.format(watched_dir=watched_dir)
    patterns = [file_path]
    print 'patterns = {patterns}'.format(patterns=', '.join(patterns))
    event_handler = MyEventHandler(patterns=patterns)
    observer = Observer()
    observer.schedule(event_handler, watched_dir, recursive=True)
    observer.start()
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        observer.stop()
        pickle.dump(model, open("C:\Users\Bartas\Documents\Unity_Projects\Neural_Network\Assets\\model.p", "wb"))
    observer.join()

if __name__ == "__main__":
    if len(sys.argv) > 1:
        path = sys.argv[1]
        main(file_path=path.strip())
    else:
        sys.exit(1)
