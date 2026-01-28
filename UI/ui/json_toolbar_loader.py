import json
from copy import copy
from typing import Union

from PyQt5.QtWidgets import QToolBar, QAction, QMenu, QWidgetAction, QToolButton

from mvc import MVC
from .json_widget_loader import JSONWidgetLoader

class JSONToolbarLoader:
    @staticmethod
    def init_toolbar(toolbar: Union[QMenu, QToolBar], data: dict, model = None, onclick = None):
        if model is None:
            model = MVC.getSyncable(data["model"]["name"])
            onclick = data["model"]["onclick"]
        for item in data["items"]:
            action = None
            if item["type"] == "action":
                action = QAction(toolbar)
                action.setText(str(item["name"]))

                if "shortcut" in item:
                    action.setShortcut(item["shortcut"])

                action_id = str(item["id"])
                if hasattr(toolbar, "json_loader_name"):
                    action_id = getattr(toolbar, "json_loader_name") + "/" + action_id
                setattr(action, "json_loader_name", action_id)
                JSONToolbarLoader._connect_click(model, onclick, action)


            elif item["type"] == "menu":
                menu = QMenu(toolbar)
                action_id = item["id"]
                if hasattr(toolbar, "json_loader_name"):
                    action_id = getattr(toolbar, "json_loader_name") + "/" + action_id
                setattr(menu, "json_loader_name", action_id)
                JSONToolbarLoader.init_toolbar(menu, item, model, onclick)

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
    def _connect_click(model, onclick, action):
        action.triggered.connect(lambda: model.Controller.Execute(onclick, getattr(action, "json_loader_name")))