FROM debian

RUN apt-get update && apt-get -y install \
                apache2 \
                libapache2-modsecurity \
                wget \
                libjansson4 \
                libcurl3 \
                libhiredis0.13 \
                libcjose0 \
                git

RUN    wget --quiet --output-document=/tmp/oidc.deb https://github.com/zmartzone/mod_auth_openidc/releases/download/v2.3.3/libapache2-mod-auth-openidc_2.3.3-1.stretch.1_amd64.deb \
    && dpkg -i /tmp/oidc.deb \
        && rm -f /tmp/oidc.deb \
        && rm -rf /var/lib/apt/lists/*

RUN a2enmod auth_openidc proxy proxy_connect ssl lbmethod_byrequests proxy_balancer proxy_http substitute headers rewrite security2 unique_id

COPY public_html /var/www
RUN cat /etc/apache2/*.conf

RUN rm /etc/apache2/*.conf
COPY *.conf /etc/apache2/

RUN mkdir /etc/apache2/modsecurity.d
RUN cd /etc/apache2/modsecurity.d; git clone https://github.com/SpiderLabs/owasp-modsecurity-crs.git
COPY crs-setup.conf  /etc/apache2/modsecurity.d/owasp-modsecurity-crs/crs-setup.conf
RUN mv /etc/apache2/modsecurity.d/owasp-modsecurity-crs/rules/REQUEST-900-EXCLUSION-RULES-BEFORE-CRS.conf.example /etc/apache2/modsecurity.d/owasp-modsecurity-crs/rules/REQUEST-900-EXCLUSION-RULES-BEFORE-CRS.conf
RUN mv /etc/apache2/modsecurity.d/owasp-modsecurity-crs/rules/RESPONSE-999-EXCLUSION-RULES-AFTER-CRS.conf.example /etc/apache2/modsecurity.d/owasp-modsecurity-crs/rules/RESPONSE-999-EXCLUSION-RULES-AFTER-CRS.conf
RUN find /etc/apache2
RUN cat /etc/apache2/conf-enabled/*.conf
ENV SERVERNAME localhost
ENV REDIS_HOSTNAME localhost
EXPOSE 80

ENTRYPOINT apache2ctl -D FOREGROUND
