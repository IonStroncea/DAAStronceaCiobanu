package org.example;

import org.example.services.Server;

import java.io.IOException;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) throws InterruptedException {
        Server server = new Server();

        Thread t = new Thread(() -> {
            try {
                server.runServer();
            } catch (IOException | InterruptedException e) {
                throw new RuntimeException(e);
            }
        });

        t.start();
        Scanner s = new Scanner(System.in);
        s.next();
       server.stop();
       t.join();
    }
}