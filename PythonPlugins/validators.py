from plugins import plugin_loadable


class Validator:
    validators = {}

    def __init__(self, _type):
        Validator.validators[_type] = type(self)

    def valid(self, widget) -> bool:
        return True

    def correct(self, widget) -> bool:
        return False
    
    def editingFinished(self, widget):
        if not self.valid(widget):
            if not self.correct(widget):
                widget.setStyleSheet("border: 2px solid #ff0000")
                widget.update()
                return
        widget.setStyleSheet("")
    
    @staticmethod
    def init_validator(json):
        if json["type"] in Validator.validators:
            return Validator.validators[json["type"]](**json)
        
    

@plugin_loadable
class UniqueValidator(Validator):
    def __init__(self, **kwargs):
        super().__init__("unique")
        self.array = kwargs["array"] if "array" in kwargs else []

    def valid(self, widget):
        return widget.text() not in self.array
