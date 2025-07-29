import inspect
import os.path


class PluginLoader:
    loaded_dependencies = {}

    @staticmethod
    def load_python_plugin(filePath, exec_env=None, only_load_dependencies=False):
        if exec_env is None:
            exec_env = {}
        with open(filePath, "r") as f:
            exec(f.read(), exec_env)
            classes = {
                obj
                for obj in exec_env.values()
                if inspect.isclass(obj)
                if hasattr(obj, "__is_loadable__")
            }

            dependency_classes = {
                obj
                for obj in exec_env.values()
                if inspect.isclass(obj)
                if hasattr(obj, "__dependencies__")
            }

            for member in dependency_classes:
                for depFile in getattr(member, "__dependencies__"):

                    # Caching dependencies speeds things up but is also necessary
                    # If this is not done, two files loading the same dependency will not agree on the members of it
                    # For example if "foo.py" is a dependency in which "class Bar" is defined,
                    # and file A and file B load it, the Bar class in file A will not be equal to the Bar class in file B,
                    # even though they are the same class and defined in the same file.
                    # Caching avoids this as now both file A and file B's Bar classes point to the same (cached) class
                    depPath = os.path.join(*os.path.split(filePath)[:-1], depFile)
                    matchedPath = PluginLoader.is_dependency_loaded(depPath)
                    if not matchedPath:
                        PluginLoader.loaded_dependencies[depPath] = {}
                        env = {}
                        PluginLoader.load_python_plugin(depPath, env, True)
                        PluginLoader.loaded_dependencies[depPath].update(env)
                        matchedPath = depPath
                    exec_env.update(PluginLoader.loaded_dependencies[matchedPath])

            if only_load_dependencies:
                return

            for plugin in classes:
                plugin()

    @staticmethod
    def is_dependency_loaded(depPath):
        for path in PluginLoader.loaded_dependencies.keys():
            if os.path.samefile(path, depPath):
                return path
        return None