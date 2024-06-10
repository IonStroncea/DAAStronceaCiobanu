package com.dnsapp.dns.services.servicesImpl;

import com.dnsapp.dns.services.AuthService;
import com.dnsapp.dns.services.RSA;
import org.springframework.stereotype.Service;

@Service
public class AuthServerImpl implements AuthService {
    @Override
    public boolean verifyToken(String token, String toFind) {
        try {
            String result = RSA.decrypt(token, RSA.getPrivateKey("src/main/resources/keyFiles/privateKey.txt"));
            return result.contains(toFind);
        } catch (Exception e) {
            return false;
        }
    }
}
