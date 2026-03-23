from System import Enum as enum
import clr


class Enum:
    @staticmethod
    def isEnum(enumType):
        if isinstance(enumType, type):
            return clr.GetClrType(enumType).IsEnum
        return enumType.IsEnum

    @staticmethod
    def getValues(enumType):
        if isinstance(enumType, type):
            return enum.GetValues[clr.GetClrType(enumType)]()
        return enum.GetValues[enumType]()

    @staticmethod
    def getValuesDict(enumType):
        values = Enum.getValues(enumType)
        return {value.ToString(): value for value in values}