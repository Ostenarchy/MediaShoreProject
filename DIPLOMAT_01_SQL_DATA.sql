USE DIPLOMAT_01
GO

-- 1. Заполнение статусов
INSERT INTO Statuses (statusName) VALUES 
(N'Выполнен'),
(N'Доставляется'),
(N'Оплачен');
GO

-- 2. Заполнение ролей
INSERT INTO Roles (roleName) VALUES 
(N'Администратор'),
(N'Менеджер'),
(N'Пользователь');
GO

-- 3. Заполнение типов дисков
INSERT INTO DiscTypes (typeName) VALUES 
(N'Кино'),
(N'Музыка'),
(N'Видеоигра'),
(N'Программное обеспечение'),
(N'Аудиокнига');
GO

-- 4. Заполнение жанров
-- Жанры для Кино (typeID = 1)
INSERT INTO DiscGenres (genreName, typeID) VALUES 
(N'Боевик', 1),
(N'Комедия', 1),
(N'Драма', 1),
(N'Ужасы', 1),
(N'Фантастика', 1),
(N'Документальное кино', 1);

-- Жанры для Музыки (typeID = 2)
INSERT INTO DiscGenres (genreName, typeID) VALUES 
(N'Рок', 2),
(N'Поп', 2),
(N'Джаз', 2),
(N'Классическая музыка', 2),
(N'Электронная музыка', 2),
(N'Хип-хоп', 2);

-- Жанры для Видеоигр (typeID = 3)
INSERT INTO DiscGenres (genreName, typeID) VALUES 
(N'Шутер', 3),
(N'Стратегия в реальном времени', 3),
(N'Рогалик', 3),
(N'RPG', 3),
(N'Гонки', 3),
(N'Симулятор', 3),
(N'Хоррор', 3);

-- Жанры для ПО (typeID = 4)
INSERT INTO DiscGenres (genreName, typeID) VALUES 
(N'Офисное ПО', 4),
(N'Графический редактор', 4),
(N'Антивирус', 4),
(N'Среда разработки', 4);

-- Жанры для Аудиокниг (typeID = 5)
INSERT INTO DiscGenres (genreName, typeID) VALUES 
(N'Роман', 5),
(N'Детектив', 5),
(N'Научная литература', 5),
(N'Фэнтези', 5);
GO

-- 5. Заполнение авторов
INSERT INTO Authors (authorFirstName, authorLastName, authorEmail) VALUES 
(N'Кристофер', N'Нолан', N'nolan@studio.com'),
(N'Квентин', N'Тарантино', N'tarantino@studio.com'),
(N'Ханс', N'Циммер', N'zimmer@music.com'),
(N'Людвиг ван', N'Бетховен', N'beethoven@classic.com'),
(N'Хидео', N'Кодзима', N'kojima@games.com'),
(N'Гейб', N'Ньюэлл', N'gaben@valve.com'),
(N'Тим', N'Бернерс-Ли', N'berners@web.com'),
(N'Стивен', N'Кинг', N'king@books.com'),
(N'Джордж', N'Мартин', N'martin@books.com'),
(N'Майкрософт', N'', N'info@microsoft.com'),
(N'Adobe', N'', N'info@adobe.com'),
(N'CD Projekt', N'RED', N'info@cdprojekt.com');
GO

-- 6. Заполнение пользователей
INSERT INTO Users (userName, userPassword, roleID, userFirstName, userLastName, userEmail, userBalance, userRegDate) VALUES 
(N'admin', N'admin123', 1, N'Иван', N'Иванов', N'admin@diplomat.ru', 50000, '2024-01-15'),
(N'manager1', N'manager123', 2, N'Петр', N'Петров', N'petrov@diplomat.ru', 30000, '2024-02-20'),
(N'user1', N'user123', 3, N'Алексей', N'Смирнов', N'smirnov@mail.ru', 15000, '2024-03-10'),
(N'user2', N'user123', 3, N'Мария', N'Кузнецова', N'kuznetsova@mail.ru', 8000, '2024-04-05'),
(N'user3', N'user123', 3, N'Дмитрий', N'Волков', N'volkov@mail.ru', 25000, '2024-05-12'),
(N'manager2', N'manager123', 2, N'Елена', N'Соколова', N'sokolova@diplomat.ru', 45000, '2024-06-01'),
(N'user4', N'user123', 3, N'Ольга', N'Морозова', N'morozova@mail.ru', 5000, '2024-07-18');
GO

-- 7. Заполнение дисков (исправленные ID жанров!)
INSERT INTO Discs (discName, discAuthorID, genreID, discQuantityInStock, discReleaseDate, discDescription, discPrice, discCoverPath) VALUES 
-- Кино (жанры ID 1-6)
(N'Начало', 1, 5, 15, '2010-07-16', N'Фантастический триллер Кристофера Нолана о внедрении в сны', 1200, N'/covers/inception.jpg'),
(N'Криминальное чтиво', 2, 1, 10, '1994-10-14', N'Культовый фильм Квентина Тарантино', 1000, N'/covers/pulp_fiction.jpg'),
(N'Сияние', 8, 4, 10, '1980-05-23', N'Экранизация романа Стивена Кинга', 900, N'/covers/shining.jpg'),

-- Музыка (жанры ID 7-12)
(N'Interstellar Soundtrack', 3, 7, 15, '2014-11-17', N'Саундтрек Ханса Циммера к фильму Интерстеллар', 1500, N'/covers/interstellar_music.jpg'),
(N'Симфония №9', 4, 10, 8, '1824-05-07', N'Девятая симфония Бетховена', 800, N'/covers/beethoven9.jpg'),
(N'Random Access Memories', NULL, 11, 20, '2013-05-17', N'Альбом Daft Punk', 1300, N'/covers/ram.jpg'),

-- Видеоигры (жанры ID 13-19)
(N'Death Stranding', 5, 13, 25, '2019-11-08', N'Инновационная игра от Хидео Кодзимы', 3500, N'/covers/death_stranding.jpg'),
(N'Half-Life 2', 6, 13, 30, '2004-11-16', N'Культовый шутер от Valve', 800, N'/covers/halflife2.jpg'),
(N'The Witcher 3: Wild Hunt', 12, 16, 20, '2015-05-19', N'Ролевая игра во вселенной Ведьмака', 2500, N'/covers/witcher3.jpg'),
(N'Cyberpunk 2077', 12, 16, 15, '2020-12-10', N'Ролевая игра в жанре киберпанк', 3000, N'/covers/cyberpunk2077.jpg'),
(N'DOOM Eternal', NULL, 13, 25, '2020-03-20', N'Шутер от первого лица', 2800, N'/covers/doom_eternal.jpg'),
(N'Civilization VI', NULL, 14, 12, '2016-10-21', N'Пошаговая стратегия', 2000, N'/covers/civ6.jpg'),
(N'Hades', NULL, 15, 30, '2020-09-17', N'Рогалик в сеттинге греческой мифологии', 1500, N'/covers/hades.jpg'),

-- ПО (жанры ID 20-23)
(N'Microsoft Office 2024', 10, 20, 10, '2024-01-15', N'Офисный пакет Microsoft', 8000, N'/covers/office2024.jpg'),
(N'Adobe Photoshop 2024', 11, 21, 5, '2024-02-10', N'Профессиональный графический редактор', 12000, N'/covers/photoshop2024.jpg'),

-- Аудиокниги (жанры ID 24-27)
(N'Оно', 8, 24, 18, '2019-09-10', N'Аудиокнига Стивена Кинга в исполнении профессионального чтеца', 1800, N'/covers/it_audiobook.jpg'),
(N'Игра престолов', 9, 27, 22, '2018-06-15', N'Первая книга саги Песнь Льда и Огня', 2000, N'/covers/got_audiobook.jpg');
GO

-- 8. Заполнение заказов
INSERT INTO Orders (userID, orderDateOfPurchase, totalSum, statusID) VALUES 
(3, '2024-08-15', 4400, 1),
(4, '2024-09-20', 5500, 1),
(5, '2024-10-05', 12000, 2),
(7, '2024-10-10', 3800, 2),
(3, '2024-11-01', 6500, 3),
(5, '2024-11-15', 15000, 3);
GO

-- 9. Заполнение позиций заказов
INSERT INTO OrderItem (orderID, discID, quantity, PriceAtTime) VALUES 
-- Заказ 1: 2x Начало (ID=1) + 1x Half-Life 2 (ID=8)
(1, 1, 2, 1200),
(1, 8, 1, 800),

-- Заказ 2: 1x Ведьмак 3 (ID=9) + 2x Симфония №9 (ID=5)
(2, 9, 1, 2500),
(2, 5, 2, 800),

-- Заказ 3: 1x Office 2024 (ID=14)
(3, 14, 1, 8000),

-- Заказ 4: 1x Оно (ID=16) + 1x Interstellar OST (ID=4)
(4, 16, 1, 1800),
(4, 4, 1, 1500),

-- Заказ 5: 1x Cyberpunk 2077 (ID=10) + 1x DOOM Eternal (ID=11)
(5, 10, 1, 3000),
(5, 11, 1, 2800),

-- Заказ 6: 1x Photoshop (ID=15) + 1x Death Stranding (ID=7)
(6, 15, 1, 12000),
(6, 7, 1, 3500);
GO