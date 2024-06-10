package com.dnsapp.dns.services;

import com.dnsapp.dns.models.Address;
import com.fasterxml.jackson.core.JsonProcessingException;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public interface DNSMainService {

    void addTranslation(String translationName, Address address);

    String getAddress(String translationName);

    void addNewImgProcessor(String role, Address address) throws JsonProcessingException;

    void addNewRegionalSender(String role, Address address) throws JsonProcessingException;

    List<String> getAllImageProcessors() throws JsonProcessingException;
    List<String> getAllRegionalSenders() throws JsonProcessingException;
}
