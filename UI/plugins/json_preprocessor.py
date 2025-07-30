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
                method_args = JSONPreprocessor.split_args(match.groups()[1]) if len(match.groups()[1]) > 0 else []
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
    def split_args(data: str):
        current: str = ""
        split = []
        in_string: bool = False

        i = 0
        while i < len(data):
            char = data[i]
            if char == '\\':
                current += char
                i += 1
                char = data[i]
            elif char == '\'' or char == '\"':
                in_string = not in_string
            elif char == ',' and not in_string:
                split.append(current)
                current = ""
                i += 1
                continue
            current += char
            i += 1

        split.append(current)
        return split

    @staticmethod
    def loads(data: str):
        loaded = json.loads(data)
        return JSONPreprocessor.preprocess(loaded)