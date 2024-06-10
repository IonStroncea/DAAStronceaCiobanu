package com.dnsapp.dns.services;

import org.springframework.stereotype.Service;

@Service
public interface AuthService {

    boolean verifyToken(String token, String toFind);
}
