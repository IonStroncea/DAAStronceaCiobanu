package com.dnsapp.dns.models;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
public class Address {
    private String ipAddress;
    private String port;

    @Override
    public String toString() {
        return this.ipAddress + ":" + this.port;
    }

}
