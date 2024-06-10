package org.example.utils;

import com.google.gson.Gson;
import lombok.AllArgsConstructor;
import lombok.Setter;
import org.example.models.PortConfigClass;

@Setter
@AllArgsConstructor
public class ConfigUtils {
    private String jsonString;

    public PortConfigClass getPortConfigClass() {
        Gson gson = new Gson();
        return gson.fromJson(jsonString, PortConfigClass.class);
    }
}
