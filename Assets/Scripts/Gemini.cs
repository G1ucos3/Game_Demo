using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini : MonoBehaviour
{
    public static Gemini Instance;

    public Action alreadyChoseWeapon;

    private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    private readonly string _apiKey = "AIzaSyAS4FQdy13XFV2PHt5Px1Agk6hENVlcQ80";

    private CurrentWeapon[] currentWeapons = new CurrentWeapon[]
    {
        new CurrentWeapon(1,"Kiếm lửa thường",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528470/0d04c180-3daa-4c62-8901-6bf5d0533964.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757481121/explose_fnf7rz.png", true),
        new CurrentWeapon(2,"Kiếm lửa ác quỷ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528499/53b8abc7-0068-483e-a21c-9e4c5a2bb363.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757481121/explose_fnf7rz.png", true),
        new CurrentWeapon(3,"Kiếm lửa siêu nhẹ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528513/5d3a3df1-e316-4e29-8d47-fd62544635c8.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(4,"Kiếm Lửa thiêng",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528754/021d8519-126c-405b-96c7-287486c2c09a.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(5,"Kiếm Lửa thần thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528832/b205c31f-9bfc-4f00-9ee4-ea608ba85e20.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(6,"Kiếm tiên quỷ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758528887/7cfaf9c6-edef-4f59-a06b-7fe31a5ad875.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(7,"Kiếm lửa anh hùng",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758529530/a58faf5d-7df0-4ecd-99bc-2d8cca261ef6.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(8,"Kiếm tộc tiên băng",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758529579/41718182-3530-4b4d-8433-1532151e358f.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(9,"Kiếm băng ngắn",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758529588/d0405715-e331-4cb3-8782-1be1c2f4f7cf.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(10,"Kiếm lửa quỷ vương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530177/3df2ea8a-881f-45ad-b381-1d9b4e0d9d6d.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(11,"Hỏa gươm đồ long đao",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530255/58b65877-86a2-4e96-afd7-30036d6b6653.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(12,"Kiếm băng đồ chơi",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530460/4c1a35a0-a4a3-434a-a117-bba90729a08b.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(13,"Kiếm băng tiên vương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530729/ddcbd87f-f1d4-4601-80f6-09a03de95afa.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(14,"Kiếm nước cơ bản",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530731/1040cd22-030a-42c8-bd05-467adbc28976.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(15,"Thủy Kiếm Vương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530789/Sword_Ice_wemk2t.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(16,"Kiếm nước tầm trung",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758530812/46a09faf-bd75-4f91-a6b6-b337925d61b6.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(17,"Kiếm mộc tầm trung",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531095/a6b07db4-0389-4e3e-941c-8020c0dd0974.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(18,"Kiếm mộc cơ bản",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531262/ba1db0f3-98eb-43c2-9f2c-3c890493995a.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(19,"Kiếm mộc siêu ngắn",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531312/8a59c4c3-6abc-4c2a-b2a8-20b3130151f1.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(20,"Kiếm tre",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531433/14a12ee1-3da8-413d-87dd-fd5baa1664b1.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(21,"Kiếm ác xà",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531572/8bd2a4e6-be2f-4611-a3ca-6c206e04dec5.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(22,"Kiếm anh hùng Mộc",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758531956/95ab4518-f153-418b-9883-a6b5100a98dd.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(23,"Kiếm siêu nhân",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758532100/66a8ecd6-2b3a-4796-9666-8155d3cb7e0c.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(24,"Dây leo gươm",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758532106/a17822b9-bf68-4d4b-a970-e03dd20b45d5.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(25,"Mộc Đao",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758532383/2f37c021-8326-47cc-ade2-41a18463aa48.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(26,"Kiếm gỗ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533123/33367e14-b90a-4664-8396-5cc55899d158.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(27,"Băng đao",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533505/670bd4c3-bd20-4536-9777-aaef6ebbf064.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(28,"Đao công nghệ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533510/63f15a40-5c8f-488b-8907-98566ad6b2bb.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(29,"Phong đao",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533716/110c5e38-160e-4c5b-9db2-25b7d938adc6.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(30,"Đại đao thủy",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533753/e58742b9-57bd-4d86-a5ad-2654c7f33930.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(31,"Hỏa gươm cơ bản",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533903/5936723b-d00c-46ef-9936-bd7b46a68d7a.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(32,"Phong gươm ngắn",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758533961/2c6a49ea-ef2f-4785-ab5f-ebc6f38ee670.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(33,"Kiếm anh hùng Phong",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534063/c4d14c64-ce4d-437f-9b45-77a51cece5e0.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(34,"Cung Băng",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534489/ad62f0ac-62ee-42d4-af6b-de769dcc49f0.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537376/download_zepyzg.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(35,"Cung Mộc",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534445/8163c619-21d3-4250-9b07-801244e87642.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534596/00afd2e4-1595-45fb-a614-f767def888d4.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(36,"Cung Hỏa",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534422/fca764a8-a1c3-458c-91e1-d2cd2929b016.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534991/2964ecb7-7849-494b-99ff-c10e3c2e5c14.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(37,"Cung Phong",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534815/6f5eb803-4a23-4343-a180-7240f1b40e18.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534971/8992bfae-5b5e-48d8-82df-ecc16891a6da.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(38,"Cung Phong thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_1_j8dpek.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(39,"Cung Hỏa thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758538354/9f94361c-301a-4070-98ee-3c237de0cefb.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758538507/download_5_bai5s3.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(40,"Cung Mộc thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758538795/e6c54a91-60c3-4168-bfe5-ad3db739742d.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758534596/00afd2e4-1595-45fb-a614-f767def888d4.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(41,"Cung Thủy thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758539206/download_7_divmby.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758539206/cung_7_kuvqcp.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(42,"Gậy Phong",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758540690/gio2_htfimd.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(43,"Gậy Băng",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758540738/download_3_xolxcv.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(44,"Gậy Hỏa Thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758540802/gay_1_jyxdos.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(45,"Gậy Hỏa",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758540799/download_1_jabvjh.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(46,"Gậy Mộc",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758541073/455c1dcb-9452-476f-9654-b3e68828aa40_l67sq8.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(47,"Gậy Phong Thánh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758540859/download_4_hnmz6l.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758537547/download_2_gbdvmr.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(48,"Gậy Bóng Chày",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707700/4678db0e-4628-47a9-8269-74e5f5da46ad.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(49,"Phong Ma Phi Tiêu",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707741/9fc221fb-7757-40f2-b641-ca9e2ab9b4d4.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(50,"Phi Tiêu Ma Pháp",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758692759/53f757ca-89cd-4056-9050-65b90e605cd1.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758692759/53f757ca-89cd-4056-9050-65b90e605cd1.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(51,"Kiếm Kim Cương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707645/download_c0n1lk.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(52,"Kiếm Âm Dương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707619/bbdfe9a1-70e4-42f7-be91-8d3d3acfa24c.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(53,"Hút Bồn Cầu Kim Cương",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707594/2_kblati.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(54,"Phi Tiêu Cây",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707570/1_ehd704.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707570/1_ehd704.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(55,"Cọc Tiêu Giao Thông",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707554/1980884b-c616-43c9-b795-9eeadc4989af.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(56,"Cây Thông Noel",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707531/3_vi1zih.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(57,"Rìu Hoàng Gia",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707513/4_j2d2fw.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(58,"Búa Sấm Sét",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707488/159f71af-1abd-40c8-a451-1512c6559fda.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(59,"Dao Phay",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707463/5_hr6yb4.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(60,"Bút Chì",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707434/6_g4jbof.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(61,"Liềm Sấm Sét",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707402/8286a469-e955-482c-b5bd-5ed4124e8301.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(62,"Liềm Tử Thần",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707366/7_r2urce.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(63,"Kiếm Bóng Đêm",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707337/8_ttvqyd.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(64,"Kiếm Nguyền Rủa",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707307/5a265899-5924-4bfd-8359-a939e028421d.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(65,"Kiếm Lửa Thần",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707270/9_ecldcg.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(66,"Kiếm Hỏa Mộc",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707242/10_rxilqs.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(67,"Kiếm Mộc Long",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707211/a532520f-8d17-4970-9829-88e5a22ca59a.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(68,"Kiếm Hoa Tử Linh",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707194/12_gbf3cj.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(69,"Kiếm Hoa Châm",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707161/11_ddftif.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(70,"Cột Điện Cao Thế",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758707140/13_ibxlez.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(71,"Cờ Crafterse",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758706960/7e66f7a4-2fea-4355-a326-5efb64129830.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(72,"Búa Thu Lôi",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758706960/7e66f7a4-2fea-4355-a326-5efb64129830.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
        new CurrentWeapon(73,"Cung Thiên Sứ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758698518/16_oiimly.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758698537/17_z3xehh.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(74,"Cung Ceasar",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758702612/bfb8efd8-a703-441f-ad2b-1bc4b6ffeaa2.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758702563/f4627050-24f3-4a3b-b602-e573ade7bc73.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(75,"Cung Xương Cá",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758698158/976536c9-097e-4889-9726-2107ba4298ad.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758698273/147b2792-39d9-431e-b97b-45e10ae36c20.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(76,"Cung Lông Vũ",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758698769/d5e03244-e1d3-47a0-8e09-4cdeaea54505.png",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758713265/a2096f7d-bb2c-4b8d-af05-17cf0ecc5a7c.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(77,"Búa DonaldTrump",
            "https://res.cloudinary.com/dlwtf6nid/image/upload/v1758713474/20_ox0t3m.png",
            "",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
    };

    public CurrentWeapon CurrentWeapon;

    [Serializable]
    public class PartReq
    {
        public string text;
    }

    [Serializable]
    public class MessageReq
    {
        public string role;
        public PartReq[] parts;
    }

    [Serializable]
    public class PropertyDetail
    {
        public string type;
        public string description;
    }

    [Serializable]
    public class PropertyDef
    {
        public PropertyDetail weaponID;
        public PropertyDetail reason;
    }

    [Serializable]
    public class ResponseSchema
    {
        public string type;
        public PropertyDef properties;
        public string[] required;
    }

    [Serializable]
    public class GenerationConfig
    {
        public string response_mime_type;
        public ResponseSchema response_schema;
    }

    [Serializable]
    public class RequestBody
    {
        public MessageReq[] contents;
        public GenerationConfig generationConfig;
    }

    [Serializable] public class TextWrapper { public string text; }
    [Serializable] public class PartsWrapper { public TextWrapper[] parts; }
    [Serializable] public class ContentWrapper { public PartsWrapper content; }
    [Serializable] public class CandidateWrapper { public ContentWrapper[] candidates; }

    [Serializable]
    public class ContentValidationResponse
    {
        public int weaponID;
        public string reason;
    }

    public void ValidateContent(string prompt, Action<ContentValidationResponse> onResult, Action<string> onError)
    {
        StartCoroutine(ValidateCoroutine(prompt, onResult, onError));
    }

    private IEnumerator ValidateCoroutine(string content, Action<ContentValidationResponse> onResult, Action<string> onError)
    {
        var weaponsList = string.Join(", ", currentWeapons.Select(w => $"{w.id}. {w.name}"));

        var msg = new MessageReq
        {
            role = "user",
            parts = new PartReq[]
            {
                new PartReq
                {
                    text = $"Hãy chọn id của weapon phù hợp nhất. Danh sách: {weaponsList}. Prompt: {content}"
                }
            }
        };

        var reqBody = new RequestBody
        {
            contents = new[] { msg },
            generationConfig = new GenerationConfig
            {
                response_mime_type = "application/json",
                response_schema = new ResponseSchema
                {
                    type = "object",
                    properties = new PropertyDef
                    {
                        weaponID = new PropertyDetail { type = "integer", description = "ID weapon phù hợp nhất" },
                        reason = new PropertyDetail { type = "string", description = "Lý do chọn" }
                    },
                    required = new[] { "weaponID", "reason" }
                }
            }
        };

        string json = JsonUtility.ToJson(reqBody);
        Debug.Log("📤 JSON gửi đi:\n" + json);

        using (UnityWebRequest www = new UnityWebRequest($"{_apiUrl}?key={_apiKey}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"Network/API Error: {www.error}\nResponse: {www.downloadHandler.text}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
            else
            {
                try
                {
                    var responseString = www.downloadHandler.text;

                    CandidateWrapper wrapper = JsonUtility.FromJson<CandidateWrapper>(responseString);
                    if (wrapper == null || wrapper.candidates == null || wrapper.candidates.Length == 0)
                    {
                        throw new Exception("Response không có candidates hợp lệ!");
                    }

                    string textJson = wrapper.candidates[0].content.parts[0].text;
                    ContentValidationResponse result = JsonUtility.FromJson<ContentValidationResponse>(textJson);

                    CurrentWeapon = currentWeapons[result.weaponID - 1];

                    if (result == null)
                    {
                        throw new Exception("Parse ContentValidationResponse thất bại!");
                    }

                    onResult?.Invoke(result);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Parse Error: " + ex.Message);
                    onError?.Invoke("Parse Error: " + ex.Message);
                }
            }
        }

        alreadyChoseWeapon?.Invoke();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}