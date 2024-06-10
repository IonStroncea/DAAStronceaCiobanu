package org.example.models;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;

@AllArgsConstructor
@Getter
@Setter
public class MessageFromRegSender {
    private String theme;
    private String ipAddress;
    private int port;
}
