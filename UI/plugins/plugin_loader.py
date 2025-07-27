import inspect
import os.path


class PluginLoader:
    @staticmethod
    def load_python_plugin(filePath):
        with open(filePath, "r") as f:
            exec_env = {}
            exec(f.read(), exec_env)
            classes = {
                obj
                for obj in exec_env.values()
                if inspect.isclass(obj)
                if getattr(obj, "__is_loadable__", False)
            }
            for plugin in classes:
                if getattr(plugin, "__dependencies__", False):
                    for depFile in getattr(plugin, "__dependencies__"):
                        with open(os.path.join(*os.path.split(filePath)[:-1], depFile)) as dep:
                            exec(dep.read(), exec_env)
                plugin()