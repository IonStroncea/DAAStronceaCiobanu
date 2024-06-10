package com.dnsapp.dns.controllers;

import com.dnsapp.dns.constans.Constant;
import com.dnsapp.dns.models.Address;
import com.dnsapp.dns.services.AuthService;
import com.dnsapp.dns.services.DNSMainService;
import com.fasterxml.jackson.core.JsonProcessingException;
import jakarta.servlet.http.HttpServletRequest;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;
import java.util.Random;


@RestController
@RequestMapping("/main")
public class MainController {

//    @Autowired
//    private HttpServletRequest context;

    @Autowired
    private DNSMainService mainService;

    @Autowired
    AuthService authService;

    @PostMapping("/addTranslation")
    public void addTranslation(String name, String port, @RequestBody String auth, HttpServletRequest rq) {
        if(authService.verifyToken(auth, Constant.ROLE_SENDER)) {
            mainService.addTranslation(name, new Address(rq.getRemoteAddr(), port));
        }
    }

    @PostMapping("/getTranslationByName")
    public String getTranslationByName(String name) {
        return mainService.getAddress(name);
    }

    @PostMapping("/getRequest")
    public int getRequest(HttpServletRequest rq) {

        rq.getRemoteAddr();

        return 0;
    }

    @PostMapping("/addNewImgProccessors")
    public void addNewImgProcessor(String port, String role,@RequestBody String auth, HttpServletRequest request) throws JsonProcessingException {
        if(authService.verifyToken(auth, Constant.ROLE_IMAGE_PROCESSOR)) {
            mainService.addNewImgProcessor(role, new Address(request.getRemoteAddr(), port));
        }
    }

    @PostMapping("/addNewRegionalSender")
    public void addNewTRegionalSender(String port,String role, String auth, HttpServletRequest request) throws JsonProcessingException {
        if (authService.verifyToken(auth, Constant.ROLE_REGIONAL_SENDER)) {
            mainService.addNewRegionalSender(role, new Address(request.getRemoteAddr(), port));
        }
    }

    @PostMapping("/getAllImageProcessors")
    public List<String> getAllImageProcessors(String auth) throws JsonProcessingException {
        if(authService.verifyToken(auth, Constant.ROLE_SENDER)) {
            return mainService.getAllImageProcessors();
        }
        return null;
    }

    @PostMapping("/getAllRegionalSenders")
    public List<String> getAllRegionalSenders(String auth) throws JsonProcessingException {
        if(authService.verifyToken(auth, Constant.ROLE_RECEIVER)) {
            return mainService.getAllRegionalSenders();
        }
        return null;
    }

    @PostMapping("/getImageProcessor")
    public String getImageProcessor(String auth) throws JsonProcessingException {
        if(authService.verifyToken(auth, Constant.ROLE_SENDER)) {
            Random random = new Random();
            List<String> result =  mainService.getAllImageProcessors();
            return result.get(random.nextInt(result.size()));
        }
        return null;
    }

    @PostMapping("/getRegionalSender")
    public String getRegionalSender(String auth) throws JsonProcessingException {
        if(authService.verifyToken(auth, Constant.ROLE_RECEIVER)) {
            Random random = new Random();
            List<String> result =  mainService.getAllRegionalSenders();
            return result.get(random.nextInt(result.size()));
        }
        return null;
    }
}