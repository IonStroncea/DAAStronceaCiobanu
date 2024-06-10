package org.example.models;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.util.Date;
import java.util.List;

@Getter
@Setter
@AllArgsConstructor
@NoArgsConstructor
public class Message {

    private String Auth;
    private String Time;
    private String Theme;
    private int X;  //number of pixels
    private int Y;
    private int[] Image; //the image stored in byte array


    public boolean isEmpty() {
        return Auth == null || Time == null || Theme == null || Image == null;
    }
}
