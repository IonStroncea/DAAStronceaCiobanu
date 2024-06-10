package org.example.services.servicesImpl;

import com.google.gson.Gson;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.Setter;
import org.example.models.Message;
import org.example.services.ImageProcessorService;

import java.io.IOException;
import java.net.*;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;

@Getter
@Setter
@AllArgsConstructor
public class UDPServer {

    boolean running = true;
    private int portFromConfig;
    private int sizeFromConfig;
    private ImageProcessorService imageProcessorService;
    Collection<String> translationNames = Collections.synchronizedCollection(new ArrayList<>());

    public UDPServer(int portFromConfig, int sizeFromConfig, ImageProcessorService imageProcessorService) {
        this.portFromConfig = portFromConfig;
        this.sizeFromConfig = sizeFromConfig;
        this.imageProcessorService = imageProcessorService;
    }

    public void run(String address, String port) {
        try {
            DatagramSocket serverSocket = new DatagramSocket(portFromConfig);
            byte[] receiveData = new byte[sizeFromConfig];
            DatagramPacket receivePacket;
            while (running) {
                //waiting for client messages
                receivePacket = new DatagramPacket(receiveData, receiveData.length);
                serverSocket.receive(receivePacket);
                String clientMessage = new String(receivePacket.getData(),0,receivePacket.getLength());
                clientMessage.trim();
                Gson g = new Gson();
                Message modelMessage = g.fromJson(clientMessage, Message.class);
                if (!modelMessage.isEmpty()) {

                    if (!translationNames.contains(modelMessage.getTheme())) {
                        translationNames.add(modelMessage.getTheme());
                        HttpClient client = HttpClient.newHttpClient();

                           HttpRequest request = HttpRequest.newBuilder()
                                .uri(URI.create(address + "/main/addTranslation?name=" + modelMessage.getTheme() + "&port=" + port))
                                .POST(HttpRequest.BodyPublishers.ofString(modelMessage.getAuth()))
                                .header("Content-Type", "application/json")
                                .build();
                        HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
                    }
                    imageProcessorService.write(modelMessage);
                }


            }
        } catch (IOException | InterruptedException e) {
            throw new RuntimeException(e);
        }
    }

    public void stopThread() {
        running = false;
    }
}
