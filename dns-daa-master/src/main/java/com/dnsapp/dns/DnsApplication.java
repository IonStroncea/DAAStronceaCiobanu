package com.dnsapp.dns;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import redis.clients.jedis.Jedis;
import redis.clients.jedis.JedisPool;

@SpringBootApplication
public class DnsApplication {

	public static void main(String[] args) {

		JedisPool pool = new JedisPool("localhost", 6379);

		Jedis jedis = pool.getResource();
		jedis.connect();
		jedis.flushAll();
		jedis.disconnect();

		SpringApplication.run(DnsApplication.class, args);
	}

}
