from PyQt5.QtWidgets import QWidget


class UI:
    all_bindings = []
    all_props = []

    @staticmethod
    def close(widget: QWidget):
        for binding in getattr(widget, "__bindings__", []):
            binding.discard()
        for child in widget.children():
            if isinstance(child, QWidget):
                UI.close(child)
        widget.deleteLater()

    @staticmethod
    def debug():
        with open("debug.txt", "w+") as f:
            f.write("Bindings:\n")
            for binding in UI.all_bindings:
                f.write(str(binding))
                f.write("\n")

            f.write("\n")
            f.write("Properties:\n")
            for i, prop in enumerate(UI.all_props):
                f.write(str(i))
                f.write(": ")
                prop.log(f)
                f.write("\n")