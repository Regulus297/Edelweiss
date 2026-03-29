import logging

from Edelweiss.Utils import EdelweissUtils
import clr

from .cs_types import Edelweiss

class CSUtils:
    @staticmethod
    def convert_type(callback):
        def inner(cs_type):
            if isinstance(cs_type, type):
                try:
                    return callback(clr.GetClrType(cs_type))
                except TypeError:
                    logging.error(f"Failed converting type {cs_type}")
                    raise
            else:
                return callback(cs_type)
        return inner

    @staticmethod
    @convert_type
    def typeIsBindableList(cs_type):
        return EdelweissUtils.IsSubclassOfRawGeneric(cs_type, Edelweiss.BindableList)

    @staticmethod
    @convert_type
    def typeIsBindableDict(cs_type):
        return EdelweissUtils.IsSubclassOfRawGeneric(cs_type, Edelweiss.BindableDictionary)