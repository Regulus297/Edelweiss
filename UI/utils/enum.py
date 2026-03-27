from System import Enum as enum
import clr

from .cs_utils import CSUtils


class Enum:
    @staticmethod
    @CSUtils.convert_type
    def isEnum(enumType):
        return enumType.IsEnum

    @staticmethod
    @CSUtils.convert_type
    def getValues(enumType):
        return enum.GetValues[enumType]()

    @staticmethod
    def getValuesDict(enumType):
        values = Enum.getValues(enumType)
        return {value.ToString(): value for value in values}