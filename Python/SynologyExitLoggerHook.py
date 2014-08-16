# Provides a hook for synology nas and writes an error event into DSM's log system

import atexit
import sys
from subprocess import call

class ExitHooks(object):
    def __init__(self):
        self.exit_code = None
        self.exception = None

    def hook(self):
        self._orig_exit = sys.exit
        sys.exit = self.exit
        sys.excepthook = self.exc_handler

    def exit(self, code=0):
        self.exit_code = code
        self._orig_exit(code)

    def exc_handler(self, exc_type, exc, *args):
        self.exception = exc
        

hooks = ExitHooks()
hooks.hook()

def synMessage(msg) :
    call(["synologset1", "sys", "err", "0x11800000", msg])

def callback():
    if hooks.exit_code is not None:
        if isinstance(hooks.exit_code, str):
            synMessage(sys.argv[0] +" exited with message: %s" % hooks.exit_code)
        else:
            synMessage(sys.argv[0] + " exited with code %d" % hooks.exit_code)
    elif hooks.exception is not None:
        synMessage(sys.argv[0] +" exited with exception: %s" % hooks.exception)

atexit.register(callback)
