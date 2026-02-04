from PyQt5.QtWidgets import QSizePolicy, QWidgetAction
from ui import MainWindow
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
        if isinstance(action, QWidgetAction) and action.defaultWidget() == MainWindow.instance.tab_switcher:
            continue
        toolbar.removeAction(action)
    toolbar.addSeparator()
    

def get_layout_item(widget):
    parent = widget.parentWidget()
    if parent is None:
        return None
    
    layout = parent.layout()
    if layout is None:
        return None
    
    index = layout.indexOf(widget)
    if index == -1:
        return None
    
    return layout.itemAt(index)


def copyJSON(json):
    if type(json) == dict:
        return {x: copyJSON(y) for x, y in json.items()}
    elif type(json) == list:
        return [copyJSON(x) for x in json]
    return json
