package org.example.services;

import com.google.gson.Gson;
import lombok.Getter;
import lombok.Setter;
import org.example.models.FullMessageFromRegSender;
import org.example.models.Message;
import org.example.models.MessageFromRegSender;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.util.*;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingDeque;
import java.util.stream.Collectors;

@Getter
@Setter
public class UdpForRegionalSender {
    BlockingQueue<Message> blockingQueue = new LinkedBlockingDeque<>();
    List<MessageFromRegSender> messageFromRegSenders = new ArrayList<>();
    private int portFromConfig;
    private int sizeFromConfig;
    private volatile boolean running = true;

    public void write(Message message) throws InterruptedException {
        blockingQueue.put(message);
    }

    public void runSend() throws InterruptedException {
        while(running) {
            Message message = blockingQueue.take();
            Gson gson = new Gson();
            String jsonMessage = gson.toJson(message);

            Set<MessageFromRegSender> messageFromRegSender = messageFromRegSenders
                    .stream()
                    .filter(m -> m.getTheme().equals(message.getTheme()))
                    .collect(Collectors.toSet());
            for (MessageFromRegSender message1 : messageFromRegSender) {
                try {
                    DatagramSocket datagramSocket = new DatagramSocket();
                    DatagramPacket sendPacket = new DatagramPacket(jsonMessage.getBytes(),
                            jsonMessage.getBytes().length,
                            InetAddress.getByName(message1.getIpAddress()), message1.getPort());
                    datagramSocket.send(sendPacket);
//                    System.out.println("Address:"+message1.getIpAddress() + ":" + message1.getPort());
//                    System.out.println("Send theme:"+message.getTheme());
//                    System.out.println("Image time:"+message.getTime());
                } catch (Exception ignored) {

                }
            }
        }

    }

    public void run() {
        while (running) {
            try {
                DatagramSocket serverSocket = new DatagramSocket(portFromConfig);
                byte[] receivedMessage = new byte[sizeFromConfig];
                DatagramPacket receivedPacket;
                while (running) {
                    receivedPacket = new DatagramPacket(receivedMessage, receivedMessage.length);
                    serverSocket.receive(receivedPacket);
                    InetAddress IPAddress = receivedPacket.getAddress();
                    int port = receivedPacket.getPort();
                    String clientMessage = new String(receivedPacket.getData(), 0, receivedPacket.getLength());
                    System.out.println(clientMessage);
                    Gson g = new Gson();
                    FullMessageFromRegSender modelMessage = g.fromJson(clientMessage, FullMessageFromRegSender.class);
                    MessageFromRegSender message = new MessageFromRegSender(modelMessage.getTheme(), IPAddress.getHostAddress(), modelMessage.getReplyPort());
                    messageFromRegSenders.add(message);
                    System.out.println("Listener :" + message.getIpAddress() + message.getPort());
                    System.out.println("Subscribed to:" + message.getTheme());
                }
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        }
    }

    public void stopThread() {
        running = false;
    }
}
