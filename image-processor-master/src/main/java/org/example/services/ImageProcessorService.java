package org.example.services;

import lombok.Getter;
import lombok.Setter;
import org.example.models.Message;

import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingDeque;

@Getter
@Setter
public class ImageProcessorService {
    boolean running = true;
    BlockingQueue<Message> blockingQueue = new LinkedBlockingDeque<>();
    private UdpForRegionalSender udpSender;

    public void write(Message message) throws InterruptedException {
        blockingQueue.put(message);
    }

    public void run() {
        while (running) {
            try {
                Message message = blockingQueue.take();
                udpSender.write(message);

            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
    }

    public void stopThread() {
        running = false;
    }
}
