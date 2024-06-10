package org.example.models;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@AllArgsConstructor
public class PortConfigClass {
    private String dnsAddress;
    private String authAddress;
    private int senderPort;
    private int regionalSenderPort;
    private int regionalListenPort;
    private int sizeFromConfig;
}
