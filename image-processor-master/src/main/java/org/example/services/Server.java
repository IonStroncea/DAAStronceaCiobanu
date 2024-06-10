package org.example.services;

import org.apache.commons.io.FileUtils;
import org.example.models.PortConfigClass;
import org.example.services.servicesImpl.AuthServerImpl;
import org.example.services.servicesImpl.UDPServer;
import org.example.utils.ConfigUtils;

import java.io.File;
import java.io.IOException;

public class Server {

    private Thread t1;
    private Thread t2;
    private Thread t3;
    private Thread t4;
    UdpForRegionalSender udpForRegionalSender;
    ImageProcessorService imageProcessorService;
    UDPServer udpServer;

    private final AuthService authService = new AuthServerImpl();

    public void runServer () throws IOException, InterruptedException {

        ConfigUtils configUtils = new ConfigUtils(FileUtils.readFileToString(new File("src/main/resources/configs/configuration.json")));
        PortConfigClass portConfigClass = configUtils.getPortConfigClass();
        String authString = authService.getAuthConnection("IMAGE_PROCESSOR", "IMAGE_PROCESSOR", portConfigClass.getAuthAddress());
        boolean connectToDNS = authService.connectToDNS("IMAGE_PROCESSOR", authString, portConfigClass.getDnsAddress(), String.valueOf(portConfigClass.getSenderPort()));

        udpForRegionalSender = new UdpForRegionalSender();
        udpForRegionalSender.setPortFromConfig(portConfigClass.getRegionalListenPort());
        udpForRegionalSender.setSizeFromConfig(portConfigClass.getSizeFromConfig());

        imageProcessorService = new ImageProcessorService();
        imageProcessorService.setUdpSender(udpForRegionalSender);

        udpServer = new UDPServer(portConfigClass.getSenderPort(), portConfigClass.getSizeFromConfig(), imageProcessorService);

        t1 = new Thread(() -> {udpForRegionalSender.run();});
        t1.start();
        t2 = new Thread(() -> {
            try {
                udpForRegionalSender.runSend();
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        });
        t2.start();
        t3 = new Thread(() -> imageProcessorService.run());
        t3.start();
        t4 = new Thread(() -> udpServer.run(portConfigClass.getDnsAddress(), String.valueOf(portConfigClass.getRegionalListenPort())));
        t4.start();

        t1.join();
        t2.join();
        t3.join();
        t4.join();
    }


    public void stop() {
        udpServer.stopThread();
        imageProcessorService.stopThread();
        udpForRegionalSender.stopThread();
    }


}
