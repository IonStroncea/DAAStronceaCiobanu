package com.dnsapp.dns.services.servicesImpl;

import com.dnsapp.dns.constans.Constant;
import com.dnsapp.dns.models.Address;
import com.dnsapp.dns.services.DNSMainService;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.gson.Gson;
import org.springframework.stereotype.Service;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.JedisPool;

import java.util.ArrayList;
import java.util.List;

@Service
public class DNSMainServiceImpl implements DNSMainService {

    JedisPool pool = new
                JedisPool("localhost", 6379);
    Jedis jedis = pool.getResource();
    @Override
    public void addTranslation(String translationName, Address address) {
        jedis.set(translationName, address.toString());
    }

    @Override
    public String getAddress(String translationName) {
        return jedis.get(translationName);
    }

    @Override
    public void addNewImgProcessor(String role, Address address) throws JsonProcessingException {

        if (jedis.get(Constant.ROLE_IMAGE_PROCESSOR) == null) {
            List<String> imgProcessors = new ArrayList<>();
            jedis.set(Constant.ROLE_IMAGE_PROCESSOR, new Gson().toJson(imgProcessors));
        }

        if (Constant.ROLE_IMAGE_PROCESSOR.equals(role)) {
            ObjectMapper mapper = new ObjectMapper();
            List<String> imgFromRedis = mapper.readValue(jedis.get(role), new TypeReference<List<String>>() {
            });
            if (!imgFromRedis.contains(address.toString()))
                imgFromRedis.add(address.toString());

            jedis.set(role, new Gson().toJson(imgFromRedis));
        }


    }

    @Override
    public void addNewRegionalSender(String role, Address address) throws JsonProcessingException {

        if (jedis.get(Constant.ROLE_REGIONAL_SENDER) == null) {
            List<String> imgProcessors = new ArrayList<>();
            jedis.set(Constant.ROLE_REGIONAL_SENDER, new Gson().toJson(imgProcessors));
        }

        if (Constant.ROLE_REGIONAL_SENDER.equals(role)) {
            ObjectMapper mapper = new ObjectMapper();
            List<String> regionalSenders = mapper.readValue(jedis.get(role), new TypeReference<List<String>>() {
            });
            if (!regionalSenders.contains(address.toString()))
                regionalSenders.add(address.toString());

            jedis.set(role, new Gson().toJson(regionalSenders));
        }

    }

    @Override
    public List<String> getAllImageProcessors() throws JsonProcessingException {
        ObjectMapper mapper = new ObjectMapper();
        check(jedis, Constant.ROLE_IMAGE_PROCESSOR);

        return mapper.readValue(jedis.get(Constant.ROLE_IMAGE_PROCESSOR), new TypeReference<List<String>>() {
        });
    }

    @Override
    public List<String> getAllRegionalSenders() throws JsonProcessingException {
        ObjectMapper mapper = new ObjectMapper();
        check(jedis, Constant.ROLE_REGIONAL_SENDER);

        return mapper.readValue(jedis.get(Constant.ROLE_REGIONAL_SENDER), new TypeReference<List<String>>() {
        });
    }

    private static void check(Jedis jedis, String role) {
        if (jedis.get(role) == null) {
            List<String> list = new ArrayList<>();
            jedis.set(role, new Gson().toJson(list));
        }
    }

}
