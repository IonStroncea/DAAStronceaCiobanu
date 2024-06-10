package org.example.services;

public interface AuthService {
    String getAuthConnection(String roleName, String roleKey, String address);
    boolean connectToDNS(String roleName, String authResponse, String address, String port);
}
