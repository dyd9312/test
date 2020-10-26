using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace address_book
{
    public partial class FrmMain : Form
    {
        private List<Address> addr;     //메모리상에 보관
        private string dir = Path.Combine(Application.StartupPath, "Myaddress.txt");
        
        public FrmMain()
        {
            InitializeComponent();
            addr = new List<Address>(); //생성자에 의해서 초기화
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            if (File.Exists(dir))       //파일이 있으면
            {
                LoadData();
            }
            this.sslCount.Text = "등록수" + addr.Count.ToString();

            if (addr.Count > 0)
            {
                ShowRecord(0);      //첫번째 데이터를 표시
            }
            btnOK.Text = "추가";
        }

        private void LoadData()
        {
            StreamReader sr = new StreamReader(dir, Encoding.Default);

            while (sr.Peek() >= 0)      //-1 = 더이상 읽을 문자가 없을때
            {
                string[] arr = sr.ReadLine().Trim().Split(','); //한줄만 읽기 전체는 ReadEnd, split는 콤마를 구분으로 나누자

                if (arr[0] != "" && arr[0] != null)
                {
                    Address a = new Address();
                    a.Num = Convert.ToInt32(arr[0]);    //번호 : 인덱스+1
                    a.Name = arr[1];
                    a.Mobile = arr[2];
                    a.Email = arr[3];

                    addr.Add(a);
                }
            }
            sr.Close();
            sr.Dispose();
            DisplayData();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            /*
            Address a = new Address();
            a.Num = addr.Count + 1;
            a.Name = txtName.Text.Trim();
            a.Mobile = txtPhone.Text.Trim();
            a.Email = txtEmail.Text.Trim();

            addr.Add(a);
            clearTextBox();
            //MessageBox.Show("완료");
            DisplayData();  //출력
            */
            if(btnOK.Text == "입력")
            {
                Address a = new Address();

                a.Num = addr.Count + 1;
                a.Name = txtName.Text.Trim();
                a.Mobile = txtPhone.Text.Trim();
                a.Email = txtEMail.Text.Trim();

                addr.Add(a);
                DisplayData();      //출력
            }
            else
            {
                ClearTextBox();
                btnOK.Text = "입력";
            }


        }

        private void ClearTextBox()
        {
            txtName.Text = txtPhone.Text = txtEMail.Text = String.Empty;
        }

        private void DisplayData()
        {
            var q = (from a in addr select a).ToList();
            this.dataGridView.DataSource = q;
        }

        //검색전용
        private void DisplayData(string query)
        {
            var q = (from a in addr
                     where
                        a.Name == query ||
                        a.Mobile == query ||
                        a.Email == query
                     select a).ToList();
            this.dataGridView.DataSource = q;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(addr.Count > 0)
            {
                SaveData();
                //MessageBox.Show("저장했습니다.");
                //MessageBox.show(Application.StartupPath + "경로에서 확인하세요");
            }
        }

        private void SaveData()
        {
            //string dir = @"D:\temp\MyAddress.txt";//생성
            //string dir = Application.StartupPath + "\\myAddress.txt"; //실행폴더에 텍스트 생성

            StringBuilder sb = new StringBuilder();

            int index = 0; //인덱스 재정렬

            foreach (Address a in addr)
            {
                sb.Append(String.Format("{0},{1},{2},{3}\r\n"
                    , ++index, a.Name, a.Mobile, a.Email));
            }
            StreamWriter sw = new StreamWriter(dir, false, Encoding.Default);
            sw.WriteLine(sb.ToString());
            sw.Close();
            sw.Dispose();
            MessageBox.Show("저장되었습니다.");

            //열고 닫고를 반복해서 한번에 설정한다 위에서
            /*
            if (!File.Exists(dir)) //파일이 존재하면
            {
                File.Create(dir);
            }

            StreamWriter sw = new StreamWriter(dir, true);
            foreach (Address a in addr)
            {
                sw.WriteLine(
                    String.Format("{0}, {1}, {2}, {3}",
                    a.Num, a.Name, a.Mobile, a.Email));
            }
            sw.Close();
            */
        }

        private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 백업ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = Path.GetFileNameWithoutExtension(dir);
            string ext = Path.GetExtension(dir).Replace(".", "");

            //MyAddress20100101.txt로 저장가능하도록
            string newDir =
                Path.Combine(Application.StartupPath,
                    String.Format("{0}{1}.{2}"
                        , name
                        , String.Format("{0}{1:0#}{2}"
                            , DateTime.Now.Year
                            , DateTime.Now.Month
                            , DateTime.Now.Day.ToString().PadLeft(2, '0')
                          )
                        , ext
                    )
                );
            if (File.Exists(dir))
            {
                File.Copy(dir, newDir, true);   //원본을 복사해서 백업
            }
        }

        private int currentIndex = -1;  //dataGridView_CellClick을 재사용하기 위해서 필드로 선언 //순서2
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //현재 선택된 인덱스 + 1을 번호 출력
            //순서1
            //this.txtNum.Text = (e.RowIndex + 1). ToString();//필드 사용전

            currentIndex = e.RowIndex; //현재 인덱스를 필드에 보관
            //202,203 검색관련해서 추가
            DataGridViewCell dgvc = dataGridView.Rows[e.RowIndex].Cells[0];
            currentIndex = Convert.ToInt32(dgvc.Value.ToString()) - 1;
            
            if (currentIndex != -1)     //순서4
            {
                //필드 사용 후 // 순서3
                //현

                //매번 재 사용하기 위해서 메서드 추출했다.
                /*
                this.txtNum.Text = (currentIndex + 1).ToString();
                this.txtName.Text = addr[currentIndex].Name;
                this.txtPhone.Text = addr[currentIndex].Mobile;
                this.txtEMail.Text = addr[currentIndex].Email;
                 */
                //ShowRecord(); 아래 ShowRecord의 매개변수를 받기에 변경
                ShowRecord(currentIndex);
            }
        }

        /*//처음, 이전, 다음, 마지막에서 사용
        private void ShowRecord()
        {
        this.txtNum.Text = (currentIndex + 1).ToString();
        this.txtName.Text = addr[currentIndex].Name;
        this.txtPhone.Text = addr[currentIndex].Mobile;
        this.txtEMail.Text = addr[currentIndex].Email;
         */
        //처음, 이전, 다음, 마지막에서 사용하기 위해 변경
        private void ShowRecord(int index)
        {
            this.txtNum.Text = (index + 1).ToString();
            this.txtName.Text = addr[index].Name;
            this.txtPhone.Text = addr[index].Mobile;
            this.txtEMail.Text = addr[index].Email;

            btnOK.Text = "추가";
            txtGo.Text = txtNum.Text;   //현재 선택된 인덱스 값 출력
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (currentIndex != -1 && blnModified==true)
            {
                //변경된 데이터로 addr 개체의 현재 인덱스 데이터 변경
                addr[currentIndex].Name = txtName.Text;
                addr[currentIndex].Mobile = txtPhone.Text;
                addr[currentIndex].Email = txtEMail.Text;
                MessageBox.Show("수정되었습니다.", "수정완료");
                DisplayData();      // 다시로드

                blnModified = false;
            }
        }
        // 3개의 텍스트박스에서 TextCharged 이벤트 발생시 호출
        private  bool blnModified = false;

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtName.Text != "")     //데이터가 로드된 상태에서만
            {
                blnModified = true;     //변경되었다.
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtNum.Text != "" && currentIndex != -1)    //현재상태
            {
                DialogResult dr = MessageBox.Show("정말로 삭제하시겠습니까?", "삭제확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.No)
                {
                    //메모리 상에서 현재 레코드 삭제
                    addr.RemoveAt(currentIndex);
                    DisplayData();
                }
            }
        }

    
        private void btnMove_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == btnFirst)
            {
                if (currentIndex > 0)
                {
                    currentIndex = 0; //0번째 인덱스로 표시
                }
            }
            else if (btn == btnPrev)
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                }
            }
            else if (btn == btnNext)
            {
                if (currentIndex < addr.Count - 1)
                {
                    currentIndex++;
                }
            }
            else if (btn == btnLast)
            {
                if (currentIndex != 1)
                {
                    currentIndex = addr.Count - 1;
                }
            }
            ShowRecord(currentIndex);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if(txtGo.Text != "" && Convert.ToInt32(txtGo.Text) > 0
                && Convert.ToInt32(txtGo.Text)<addr.Count)
            {
                ShowRecord(Convert.ToInt32(txtGo.Text) - 1);
            }
        }

        private void btnSerch_Click(object sender, EventArgs e)
        {
            DisplayData(txtSerch.Text);
        }

        
        
        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /*
        private void miBackUp_Click(object sender, EventArgs e)
        {
            string name = Path.GetFileNameWithoutExtension(dir);
            string ext = Path.GetExtension(dir).Replace(".", "");

            string newDir = Path.Combine(Application.StartupPath,
                string.Format("{0}{1}{2}", name, String.Format("{0}{1:}#{2}",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day.ToString().PadLeft(2, '0'),
                ext)));
            
            if (File.Exists(dir))
            {
                File.Copy(dir, newDir, true);
            }
        }
        */
    }
}
