package org.example.services.servicesImpl;

import com.google.gson.Gson;
import lombok.AllArgsConstructor;
import org.example.services.AuthService;

import java.net.URI;
import java.net.http.*;

public class AuthServerImpl implements AuthService {

    @Override
    public String getAuthConnection(String roleName, String roleKey, String address) {
        MessageModel messageModel = new MessageModel(roleName, roleKey);
        Gson gson = new Gson();
        String jsonToSend = gson.toJson(messageModel);

        try {
            HttpClient client = HttpClient.newHttpClient();
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(address + "/LoginType"))
                    .POST(HttpRequest.BodyPublishers.ofString(jsonToSend))
                    .header("Content-Type", "application/json")
                    .build();

            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
            return response.body();
        } catch (Exception ignored) {

        }
        return "";
    }

    @Override
    public boolean connectToDNS(String roleName, String authResponse, String address, String port) {
        MessageModelDNS message = new MessageModelDNS(authResponse);

        try {
            HttpClient client = HttpClient.newHttpClient();
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(address + "/main/addNewImgProccessors?port="+ port +"&role=" + roleName))
                    .POST(HttpRequest.BodyPublishers.ofString(authResponse))
                    .build();

            HttpResponse<String> response = client.send(request, HttpResponse.BodyHandlers.ofString());
            return true;
        } catch (Exception ignored) {

        }
        return true;
    }


    @AllArgsConstructor
    private static class MessageModel {
        String TypeName;
        String Key;
    }

    @AllArgsConstructor
    private static class MessageModelDNS {
        String Auth;
    }
}
