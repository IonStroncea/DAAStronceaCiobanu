package org.example.models;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

@Setter
@Getter
@AllArgsConstructor
public class Address {
    private String ipAddress;
    private String port;

    @Override
    public String toString() {
        return this.ipAddress + ":" + this.port;
    }
}
