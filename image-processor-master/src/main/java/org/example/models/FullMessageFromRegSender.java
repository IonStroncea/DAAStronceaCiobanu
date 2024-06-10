package org.example.models;

import lombok.AllArgsConstructor;
import lombok.Getter;

@Getter
@AllArgsConstructor
public class FullMessageFromRegSender {
    private String Theme;
    private int ReplyPort;
    private String Auth;

}
