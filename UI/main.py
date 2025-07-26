import os.path

if not os.path.exists(os.path.join(os.getcwd(), "Build/Edelweiss.dll")):
    os.chdir("../")


from network.network_manager import PyNetworkManager

PyNetworkManager.initialize()

while not PyNetworkManager.halt:
    PyNetworkManager.update()

PyNetworkManager.exit()