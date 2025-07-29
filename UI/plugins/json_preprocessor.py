import json
import re

from .util_json_function import UtilJsonFunction


class JSONPreprocessor:
    pattern = ""

    @staticmethod
    def preprocess(data):
        if type(data) == str:
            match = re.match(r"@(\w+)\((.*)\)", data)
            if match:
                method_name = match.groups()[0]
                method_args = match.groups()[1].split(",")
                method_args = [eval(arg) for arg in method_args]
                if method_name not in UtilJsonFunction.functions.keys():
                    print(f"Unrecognized function: {method_name} called as {data}")
                    return data
                return UtilJsonFunction.functions[method_name](*method_args)
            return data
        elif type(data) == dict:
            for key, value in data.items():
                data[key] = JSONPreprocessor.preprocess(value)
            return data
        elif type(data) == list:
            return [JSONPreprocessor.preprocess(item) for item in data]
        return data



    @staticmethod
    def loads(data: str):
        loaded = json.loads(data)
        return JSONPreprocessor.preprocess(loaded)