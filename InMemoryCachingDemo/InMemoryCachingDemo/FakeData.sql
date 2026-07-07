/* =========================
   INSERT COUNTRY
   ========================= */

INSERT INTO Country (Name, NameOfVietNamese)
VALUES
(N'Vietnam', N'Việt Nam'),
(N'United States', N'Hoa Kỳ'),
(N'United Kingdom', N'Vương quốc Anh'),
(N'France', N'Pháp'),
(N'Germany', N'Đức'),
(N'Italy', N'Ý'),
(N'Spain', N'Tây Ban Nha'),
(N'Japan', N'Nhật Bản'),
(N'South Korea', N'Hàn Quốc'),
(N'China', N'Trung Quốc'),
(N'Thailand', N'Thái Lan'),
(N'Singapore', N'Singapore'),
(N'Malaysia', N'Malaysia'),
(N'Indonesia', N'Indonesia'),
(N'Philippines', N'Philippines'),
(N'India', N'Ấn Độ'),
(N'Australia', N'Úc'),
(N'Canada', N'Canada'),
(N'Brazil', N'Brazil'),
(N'Russia', N'Nga'),
(N'Netherlands', N'Hà Lan'),
(N'Switzerland', N'Thụy Sĩ'),
(N'Sweden', N'Thụy Điển'),
(N'Norway', N'Na Uy'),
(N'Denmark', N'Đan Mạch');


/* =========================
   INSERT CITY
   ========================= */

INSERT INTO City (CountryId, Name)
SELECT c.Id, v.CityName
FROM Country c
JOIN (
    VALUES
    -- Vietnam
    (N'Vietnam', N'Ha Noi'),
    (N'Vietnam', N'Ho Chi Minh City'),
    (N'Vietnam', N'Da Nang'),
    (N'Vietnam', N'Hai Phong'),
    (N'Vietnam', N'Can Tho'),
    (N'Vietnam', N'Hue'),
    (N'Vietnam', N'Nha Trang'),
    (N'Vietnam', N'Da Lat'),
    (N'Vietnam', N'Vung Tau'),
    (N'Vietnam', N'Quy Nhon'),

    -- United States
    (N'United States', N'New York'),
    (N'United States', N'Los Angeles'),
    (N'United States', N'Chicago'),
    (N'United States', N'Houston'),
    (N'United States', N'Phoenix'),
    (N'United States', N'Philadelphia'),
    (N'United States', N'San Antonio'),
    (N'United States', N'San Diego'),
    (N'United States', N'Dallas'),
    (N'United States', N'San Jose'),

    -- United Kingdom
    (N'United Kingdom', N'London'),
    (N'United Kingdom', N'Manchester'),
    (N'United Kingdom', N'Birmingham'),
    (N'United Kingdom', N'Liverpool'),
    (N'United Kingdom', N'Leeds'),
    (N'United Kingdom', N'Glasgow'),
    (N'United Kingdom', N'Edinburgh'),
    (N'United Kingdom', N'Bristol'),

    -- France
    (N'France', N'Paris'),
    (N'France', N'Marseille'),
    (N'France', N'Lyon'),
    (N'France', N'Toulouse'),
    (N'France', N'Nice'),
    (N'France', N'Nantes'),
    (N'France', N'Strasbourg'),
    (N'France', N'Bordeaux'),

    -- Germany
    (N'Germany', N'Berlin'),
    (N'Germany', N'Hamburg'),
    (N'Germany', N'Munich'),
    (N'Germany', N'Cologne'),
    (N'Germany', N'Frankfurt'),
    (N'Germany', N'Stuttgart'),
    (N'Germany', N'Dusseldorf'),
    (N'Germany', N'Dortmund'),

    -- Italy
    (N'Italy', N'Rome'),
    (N'Italy', N'Milan'),
    (N'Italy', N'Naples'),
    (N'Italy', N'Turin'),
    (N'Italy', N'Palermo'),
    (N'Italy', N'Genoa'),
    (N'Italy', N'Bologna'),
    (N'Italy', N'Florence'),

    -- Spain
    (N'Spain', N'Madrid'),
    (N'Spain', N'Barcelona'),
    (N'Spain', N'Valencia'),
    (N'Spain', N'Seville'),
    (N'Spain', N'Zaragoza'),
    (N'Spain', N'Malaga'),
    (N'Spain', N'Murcia'),
    (N'Spain', N'Bilbao'),

    -- Japan
    (N'Japan', N'Tokyo'),
    (N'Japan', N'Osaka'),
    (N'Japan', N'Kyoto'),
    (N'Japan', N'Yokohama'),
    (N'Japan', N'Nagoya'),
    (N'Japan', N'Sapporo'),
    (N'Japan', N'Fukuoka'),
    (N'Japan', N'Kobe'),

    -- South Korea
    (N'South Korea', N'Seoul'),
    (N'South Korea', N'Busan'),
    (N'South Korea', N'Incheon'),
    (N'South Korea', N'Daegu'),
    (N'South Korea', N'Daejeon'),
    (N'South Korea', N'Gwangju'),
    (N'South Korea', N'Suwon'),
    (N'South Korea', N'Ulsan'),

    -- China
    (N'China', N'Beijing'),
    (N'China', N'Shanghai'),
    (N'China', N'Guangzhou'),
    (N'China', N'Shenzhen'),
    (N'China', N'Chengdu'),
    (N'China', N'Hangzhou'),
    (N'China', N'Wuhan'),
    (N'China', N'Xi An'),

    -- Thailand
    (N'Thailand', N'Bangkok'),
    (N'Thailand', N'Chiang Mai'),
    (N'Thailand', N'Phuket'),
    (N'Thailand', N'Pattaya'),
    (N'Thailand', N'Krabi'),
    (N'Thailand', N'Ayutthaya'),

    -- Singapore
    (N'Singapore', N'Singapore'),

    -- Malaysia
    (N'Malaysia', N'Kuala Lumpur'),
    (N'Malaysia', N'George Town'),
    (N'Malaysia', N'Johor Bahru'),
    (N'Malaysia', N'Malacca'),
    (N'Malaysia', N'Kota Kinabalu'),
    (N'Malaysia', N'Ipoh'),

    -- Indonesia
    (N'Indonesia', N'Jakarta'),
    (N'Indonesia', N'Surabaya'),
    (N'Indonesia', N'Bandung'),
    (N'Indonesia', N'Medan'),
    (N'Indonesia', N'Bali'),
    (N'Indonesia', N'Yogyakarta'),

    -- Philippines
    (N'Philippines', N'Manila'),
    (N'Philippines', N'Quezon City'),
    (N'Philippines', N'Cebu City'),
    (N'Philippines', N'Davao City'),
    (N'Philippines', N'Makati'),
    (N'Philippines', N'Pasig'),

    -- India
    (N'India', N'New Delhi'),
    (N'India', N'Mumbai'),
    (N'India', N'Bangalore'),
    (N'India', N'Hyderabad'),
    (N'India', N'Chennai'),
    (N'India', N'Kolkata'),
    (N'India', N'Pune'),
    (N'India', N'Ahmedabad'),

    -- Australia
    (N'Australia', N'Sydney'),
    (N'Australia', N'Melbourne'),
    (N'Australia', N'Brisbane'),
    (N'Australia', N'Perth'),
    (N'Australia', N'Adelaide'),
    (N'Australia', N'Canberra'),
    (N'Australia', N'Gold Coast'),

    -- Canada
    (N'Canada', N'Toronto'),
    (N'Canada', N'Vancouver'),
    (N'Canada', N'Montreal'),
    (N'Canada', N'Calgary'),
    (N'Canada', N'Ottawa'),
    (N'Canada', N'Edmonton'),
    (N'Canada', N'Quebec City'),

    -- Brazil
    (N'Brazil', N'Sao Paulo'),
    (N'Brazil', N'Rio de Janeiro'),
    (N'Brazil', N'Brasilia'),
    (N'Brazil', N'Salvador'),
    (N'Brazil', N'Fortaleza'),
    (N'Brazil', N'Curitiba'),

    -- Russia
    (N'Russia', N'Moscow'),
    (N'Russia', N'Saint Petersburg'),
    (N'Russia', N'Novosibirsk'),
    (N'Russia', N'Yekaterinburg'),
    (N'Russia', N'Kazan'),
    (N'Russia', N'Sochi'),

    -- Netherlands
    (N'Netherlands', N'Amsterdam'),
    (N'Netherlands', N'Rotterdam'),
    (N'Netherlands', N'The Hague'),
    (N'Netherlands', N'Utrecht'),
    (N'Netherlands', N'Eindhoven'),

    -- Switzerland
    (N'Switzerland', N'Zurich'),
    (N'Switzerland', N'Geneva'),
    (N'Switzerland', N'Basel'),
    (N'Switzerland', N'Bern'),
    (N'Switzerland', N'Lausanne'),

    -- Sweden
    (N'Sweden', N'Stockholm'),
    (N'Sweden', N'Gothenburg'),
    (N'Sweden', N'Malmo'),
    (N'Sweden', N'Uppsala'),
    (N'Sweden', N'Vasteras'),

    -- Norway
    (N'Norway', N'Oslo'),
    (N'Norway', N'Bergen'),
    (N'Norway', N'Trondheim'),
    (N'Norway', N'Stavanger'),
    (N'Norway', N'Tromso'),

    -- Denmark
    (N'Denmark', N'Copenhagen'),
    (N'Denmark', N'Aarhus'),
    (N'Denmark', N'Odense'),
    (N'Denmark', N'Aalborg'),
    (N'Denmark', N'Esbjerg')
) v(CountryName, CityName)
ON c.Name = v.CountryName;