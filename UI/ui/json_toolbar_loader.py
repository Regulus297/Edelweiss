from typing import Union

from PyQt5.QtWidgets import QToolBar, QAction, QMenu

from .localized_binding import LocalizedBinding
from .json_widget_loader import JSONWidgetLoader
from .widget_method import WidgetMethod


class JSONToolbarLoader:
    @staticmethod
    def init_toolbar(toolbar: Union[QMenu, QToolBar], data: dict):
        for item in data["items"]:
            action = None
            if item["type"] == "action":
                action = QAction(toolbar)
                LocalizedBinding(action, item, "name", ValueChanged=action.setText)
                WidgetMethod.create(action, action.triggered, item, "click")

                if "shortcut" in item:
                    action.setShortcut(item["shortcut"])

            elif item["type"] == "menu":
                menu = QMenu(toolbar)
                JSONToolbarLoader.init_toolbar(menu, item)

                action = QAction(toolbar)
                action.setMenu(menu)
                LocalizedBinding(action, item, "name", ValueChanged=action.setText)
            elif item["type"] == "widget":
                if isinstance(toolbar, QMenu):
                    print("Putting widgets inside menus is not supported")
                    continue
                toolbar.addWidget(JSONWidgetLoader.init_widget(item["data"], toolbar))
                continue

            if action is None:
                print(f"Unrecognized item type {item['type']}")
                return

            toolbar.addAction(action)
