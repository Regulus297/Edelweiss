from PyQt5.QtWidgets import QSizePolicy, QWidgetAction
from ui import MappingWindow
from plugins import load_dependencies


class CommonVars:
    found_submit_button = None


def get_size_policy(policy: str) -> QSizePolicy:
    if policy == "Expanding":
        return QSizePolicy.Expanding
    elif policy == "Preferred":
        return QSizePolicy.Preferred

    return  QSizePolicy.Fixed

def clear_main_toolbar(toolbar):
    for action in toolbar.actions():
        if isinstance(action, QWidgetAction) and action.defaultWidget() == MappingWindow.instance.tab_switcher:
            continue
        toolbar.removeAction(action)
    toolbar.addSeparator()


# Here we have the first time anyone used __new__ in recorded history
@load_dependencies("deferred_value.py")
class value:
    def __new__(self, val):
        if isinstance(val, DeferredValue):
            return val._func()
        return val