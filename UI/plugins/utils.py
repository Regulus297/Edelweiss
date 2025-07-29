def get_extra_data_safe(data):
    if "extraData" not in data.keys():
        return {}

    extra_data = {}
    for key, value in data["extraData"].items():
        if isinstance(value, dict):
            cast_to_type = str
            if value["type"] == "int":
                cast_to_type = int
            elif value["type"] == "float":
                cast_to_type = float
            elif value["type"] == "bool":
                cast_to_type = bool
            extra_data[key] = cast_to_type(value["value"])
        else:
            extra_data[key] = str(value)
    return extra_data
