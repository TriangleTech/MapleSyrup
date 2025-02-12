PGDMP                      }         
   maplesyrup    17.2    17.2 %    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            �           1262    16388 
   maplesyrup    DATABASE     �   CREATE DATABASE maplesyrup WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1252';
    DROP DATABASE maplesyrup;
                     postgres    false                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
                     pg_database_owner    false            �           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                        pg_database_owner    false    4            �            1259    16390    accounts    TABLE     *  CREATE TABLE public.accounts (
    account_id integer NOT NULL,
    account_user character(32) NOT NULL,
    account_pass character(32) NOT NULL,
    account_status smallint DEFAULT 0 NOT NULL,
    character_count smallint DEFAULT 0 NOT NULL,
    max_character_slots smallint DEFAULT 3 NOT NULL
);
    DROP TABLE public.accounts;
       public         heap r       postgres    false    4            �            1259    16389    accounts_account_id_seq    SEQUENCE     �   CREATE SEQUENCE public.accounts_account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 .   DROP SEQUENCE public.accounts_account_id_seq;
       public               postgres    false    218    4            �           0    0    accounts_account_id_seq    SEQUENCE OWNED BY     S   ALTER SEQUENCE public.accounts_account_id_seq OWNED BY public.accounts.account_id;
          public               postgres    false    217            �            1259    16448 	   equipment    TABLE     �   CREATE TABLE public.equipment (
    account_id integer NOT NULL,
    character_id integer NOT NULL,
    item_type integer NOT NULL,
    item_id integer NOT NULL
);
    DROP TABLE public.equipment;
       public         heap r       postgres    false    4            �            1259    16446 "   character_equipment_account_id_seq    SEQUENCE     �   CREATE SEQUENCE public.character_equipment_account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 9   DROP SEQUENCE public.character_equipment_account_id_seq;
       public               postgres    false    4    224            �           0    0 "   character_equipment_account_id_seq    SEQUENCE OWNED BY     _   ALTER SEQUENCE public.character_equipment_account_id_seq OWNED BY public.equipment.account_id;
          public               postgres    false    222            �            1259    16447 $   character_equipment_character_id_seq    SEQUENCE     �   CREATE SEQUENCE public.character_equipment_character_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 ;   DROP SEQUENCE public.character_equipment_character_id_seq;
       public               postgres    false    224    4            �           0    0 $   character_equipment_character_id_seq    SEQUENCE OWNED BY     c   ALTER SEQUENCE public.character_equipment_character_id_seq OWNED BY public.equipment.character_id;
          public               postgres    false    223            �            1259    16415 
   characters    TABLE     �  CREATE TABLE public.characters (
    character_id integer NOT NULL,
    account_id integer NOT NULL,
    spawn_map character(9) DEFAULT 100000000 NOT NULL,
    skin_id smallint DEFAULT 0 NOT NULL,
    character_name character varying(32) NOT NULL,
    character_level integer DEFAULT 1 NOT NULL,
    character_str integer DEFAULT 4 NOT NULL,
    character_dex integer DEFAULT 4 NOT NULL,
    character_int integer DEFAULT 4 NOT NULL,
    character_luk integer DEFAULT 4 NOT NULL,
    character_hp integer DEFAULT 50 NOT NULL,
    character_mp integer DEFAULT 100 NOT NULL,
    character_max_hp integer DEFAULT 50 NOT NULL,
    character_max_mp integer DEFAULT 100 NOT NULL
);
    DROP TABLE public.characters;
       public         heap r       postgres    false    4            �            1259    16414    characters_account_id_seq    SEQUENCE     �   CREATE SEQUENCE public.characters_account_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 0   DROP SEQUENCE public.characters_account_id_seq;
       public               postgres    false    4    221            �           0    0    characters_account_id_seq    SEQUENCE OWNED BY     W   ALTER SEQUENCE public.characters_account_id_seq OWNED BY public.characters.account_id;
          public               postgres    false    220            �            1259    16413    characters_character_id_seq    SEQUENCE     �   CREATE SEQUENCE public.characters_character_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 2   DROP SEQUENCE public.characters_character_id_seq;
       public               postgres    false    4    221            �           0    0    characters_character_id_seq    SEQUENCE OWNED BY     [   ALTER SEQUENCE public.characters_character_id_seq OWNED BY public.characters.character_id;
          public               postgres    false    219            -           2604    16393    accounts account_id    DEFAULT     z   ALTER TABLE ONLY public.accounts ALTER COLUMN account_id SET DEFAULT nextval('public.accounts_account_id_seq'::regclass);
 B   ALTER TABLE public.accounts ALTER COLUMN account_id DROP DEFAULT;
       public               postgres    false    218    217    218            1           2604    16418    characters character_id    DEFAULT     �   ALTER TABLE ONLY public.characters ALTER COLUMN character_id SET DEFAULT nextval('public.characters_character_id_seq'::regclass);
 F   ALTER TABLE public.characters ALTER COLUMN character_id DROP DEFAULT;
       public               postgres    false    219    221    221            2           2604    16419    characters account_id    DEFAULT     ~   ALTER TABLE ONLY public.characters ALTER COLUMN account_id SET DEFAULT nextval('public.characters_account_id_seq'::regclass);
 D   ALTER TABLE public.characters ALTER COLUMN account_id DROP DEFAULT;
       public               postgres    false    221    220    221            >           2604    16451    equipment account_id    DEFAULT     �   ALTER TABLE ONLY public.equipment ALTER COLUMN account_id SET DEFAULT nextval('public.character_equipment_account_id_seq'::regclass);
 C   ALTER TABLE public.equipment ALTER COLUMN account_id DROP DEFAULT;
       public               postgres    false    222    224    224            ?           2604    16452    equipment character_id    DEFAULT     �   ALTER TABLE ONLY public.equipment ALTER COLUMN character_id SET DEFAULT nextval('public.character_equipment_character_id_seq'::regclass);
 E   ALTER TABLE public.equipment ALTER COLUMN character_id DROP DEFAULT;
       public               postgres    false    223    224    224            �          0    16390    accounts 
   TABLE DATA           �   COPY public.accounts (account_id, account_user, account_pass, account_status, character_count, max_character_slots) FROM stdin;
    public               postgres    false    218   .       �          0    16415 
   characters 
   TABLE DATA           �   COPY public.characters (character_id, account_id, spawn_map, skin_id, character_name, character_level, character_str, character_dex, character_int, character_luk, character_hp, character_mp, character_max_hp, character_max_mp) FROM stdin;
    public               postgres    false    221   1.       �          0    16448 	   equipment 
   TABLE DATA           Q   COPY public.equipment (account_id, character_id, item_type, item_id) FROM stdin;
    public               postgres    false    224   N.       �           0    0    accounts_account_id_seq    SEQUENCE SET     F   SELECT pg_catalog.setval('public.accounts_account_id_seq', 1, false);
          public               postgres    false    217            �           0    0 "   character_equipment_account_id_seq    SEQUENCE SET     Q   SELECT pg_catalog.setval('public.character_equipment_account_id_seq', 1, false);
          public               postgres    false    222            �           0    0 $   character_equipment_character_id_seq    SEQUENCE SET     S   SELECT pg_catalog.setval('public.character_equipment_character_id_seq', 1, false);
          public               postgres    false    223            �           0    0    characters_account_id_seq    SEQUENCE SET     H   SELECT pg_catalog.setval('public.characters_account_id_seq', 1, false);
          public               postgres    false    220            �           0    0    characters_character_id_seq    SEQUENCE SET     J   SELECT pg_catalog.setval('public.characters_character_id_seq', 1, false);
          public               postgres    false    219            A           2606    16398    accounts accounts_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT accounts_pkey PRIMARY KEY (account_id);
 @   ALTER TABLE ONLY public.accounts DROP CONSTRAINT accounts_pkey;
       public                 postgres    false    218            C           2606    16432    characters characters_pkey 
   CONSTRAINT     b   ALTER TABLE ONLY public.characters
    ADD CONSTRAINT characters_pkey PRIMARY KEY (character_id);
 D   ALTER TABLE ONLY public.characters DROP CONSTRAINT characters_pkey;
       public                 postgres    false    221            D           2606    16433    characters account_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.characters
    ADD CONSTRAINT account_id FOREIGN KEY (account_id) REFERENCES public.accounts(account_id);
 ?   ALTER TABLE ONLY public.characters DROP CONSTRAINT account_id;
       public               postgres    false    218    221    4673            E           2606    16453    equipment account_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT account_id FOREIGN KEY (account_id) REFERENCES public.accounts(account_id);
 >   ALTER TABLE ONLY public.equipment DROP CONSTRAINT account_id;
       public               postgres    false    224    218    4673            F           2606    16458    equipment character_id    FK CONSTRAINT     �   ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT character_id FOREIGN KEY (character_id) REFERENCES public.characters(character_id);
 @   ALTER TABLE ONLY public.equipment DROP CONSTRAINT character_id;
       public               postgres    false    224    221    4675            �      x������ � �      �      x������ � �      �      x������ � �     