import inspect
import os.path



class PluginLoader:
    loaded_dependencies = {}
    plugins_var = None

    @staticmethod
    def bind():
        from interop import SyncableProperty
        from interop.dict_binding import DictBinding
        PluginLoader.plugins_var = SyncableProperty("Edelweiss.PythonPlugins")
        DictBinding(PluginLoader.plugins_var, PluginLoader.load_plugins, lambda x : None, lambda x, _ : PluginLoader.load_python_plugin(x), lambda x: None)

    @staticmethod
    def load_plugins(plugins):
        for pair in plugins:
            PluginLoader.load_python_plugin(pair.Value)

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
                for depKey in getattr(member, "__dependencies__"):

                    # Caching dependencies speeds things up but is also necessary
                    # If this is not done, two files loading the same dependency will not agree on the members of it
                    # For example if "foo.py" is a dependency in which "class Bar" is defined,
                    # and file A and file B load it, the Bar class in file A will not be equal to the Bar class in file B,
                    # even though they are the same class and defined in the same file.
                    # Caching avoids this as now both file A and file B's Bar classes point to the same (cached) class
                    depPath = PluginLoader.plugins_var.get()[depKey]
                    if depKey not in PluginLoader.loaded_dependencies:
                        PluginLoader.loaded_dependencies[depKey] = {}
                        env = {}
                        PluginLoader.load_python_plugin(depPath, env, not getattr(member, "__load_types__", False))
                        PluginLoader.loaded_dependencies[depKey].update(env)
                    exec_env.update(PluginLoader.loaded_dependencies[depKey])

            if only_load_dependencies:
                return

            for plugin in classes:
                plugin()
