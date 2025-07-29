import json
from copy import copy
from typing import Union

from PyQt5.QtWidgets import QToolBar, QAction, QMenu, QWidgetAction, QToolButton

from network import PyNetworkManager
from plugins import get_extra_data_safe
from .json_widget_loader import JSONWidgetLoader
from .main_window import MappingWindow


class JSONToolbarLoader:
    @staticmethod
    def init_toolbar(toolbar: Union[QMenu, QToolBar], data: dict, clear = lambda toolbar: toolbar.clear()):
        clear(toolbar)
        for item in data["items"]:
            action = None
            if item["type"] == "action":
                action = QAction(toolbar)
                action.setText(item["name"])

                action_name = item["name"]
                if hasattr(toolbar, "json_loader_name"):
                    action_name = getattr(toolbar, "json_loader_name") + "/" + action_name
                setattr(action, "json_loader_name", action_name)

                if "onclick" in item.keys():
                    JSONToolbarLoader.register_onclick(action, item)

            elif item["type"] == "menu":
                menu = QMenu(toolbar)
                action_name = item["name"]
                if hasattr(toolbar, "json_loader_name"):
                    action_name = getattr(toolbar, "json_loader_name") + "/" + action_name
                setattr(menu, "json_loader_name", action_name)
                JSONToolbarLoader.init_toolbar(menu, item)

                action = QAction(toolbar)
                action.setText(item["name"])
                action.setMenu(menu)
            elif item["type"] == "widget":
                if toolbar is QMenu:
                    print("Putting widgets inside menus is not supported")
                    continue
                toolbar.addWidget(JSONWidgetLoader.init_widget(item["data"], toolbar))
                continue

            if action is None:
                print(f"Unrecognized item type {item['type']}")
                return

            toolbar.addAction(action)

    @staticmethod
    def register_onclick(action, data):
        action.triggered.connect(lambda: PyNetworkManager.send_packet(data["onclick"], json.dumps({
            "tab": MappingWindow.instance.current_tab,
            "name": getattr(action, "json_loader_name"),
            "extraData": get_extra_data_safe(data)
        })))