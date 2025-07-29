from PyQt5.QtWidgets import QSizePolicy, QWidgetAction
from ui import MappingWindow


class CommonVars:
    found_submit_button = None


def get_size_policy(policy: str) -> QSizePolicy:
    if policy == "Expanding":
        return QSizePolicy.Expanding
    elif policy == "Preferred":
        return QSizePolicy.Preferred

    return  QSizePolicy.Fixed

def clear_main_toolbar(toolbar):
    action = QWidgetAction(toolbar)
    action.setDefaultWidget(MappingWindow.instance.tab_switcher)
    toolbar.clear()
    toolbar.addAction(action)
    toolbar.addSeparator()