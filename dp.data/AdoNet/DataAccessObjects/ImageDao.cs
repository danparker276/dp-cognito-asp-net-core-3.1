using dp.business.Helpers;
using dp.data.AdoNet.SqlExecution;
using System;
using System.Data;
using System.Threading.Tasks;

namespace dp.data.AdoNet.DataAccessObjects
{
    public class ImageDao : BaseDao
    {
        public ImageDao(string dpDbConnectionString) : base(dpDbConnectionString)
        {
        }


        public async Task<string> AddImageAsync(byte[] imageIn, int userId)
        {
            byte[] imageData = ImageHelper.CreateImageThumbnail(imageIn, true, 1000, 1000); //don't know if we always want a small pic
                                                                                            //right now just saving to jpg
                                                                                            //  byte[] imageThumbData = ImageHelper.CreateImageThumbnail(model.ImageData, 200, 200);

            byte[] imageThumb = ImageHelper.CreateImageThumbnail(imageIn, false, 50, 50);
            SqlQuery proc = new SqlQuery(@" 
insert into images (ImageData, ImageThumb, UserId)
OUTPUT inserted.ImageGUID 
values(@imageData,@imageThumb, @userId)
;
                ", 30, System.Data.CommandType.Text);

            proc.AddInputParam("ImageData", SqlDbType.Image, imageData);
            proc.AddInputParam("ImageThumb", SqlDbType.Image, imageThumb);
            proc.AddInputParam("UserId", SqlDbType.NVarChar, userId);
            return await _queryExecutor.ExecuteAsync(proc, sqlReader => GetReturnValue<string>(sqlReader));


        }

        private string AddImageAsyncResult(IDataReader oReader)
        {
            oReader.Read();
            return SqlQueryResultParser.GetValue<string>(oReader, "imageGUID");
        }


        public async Task<byte[]> GetImageAsync(string imageGUID, bool isThumb)
        {

            string sql = " SELECT ImageData  FROM Images WHERE[ImageGUID] = @imageGUID;";
            if (isThumb)
            {
                sql = " SELECT ImageThumb as ImageData FROM Images WHERE[ImageGUID] = @imageGUID;";

            }
            SqlQuery proc = new SqlQuery(sql, 30, System.Data.CommandType.Text);



            proc.AddInputParam("imageGUID", SqlDbType.VarChar, imageGUID);


            var procedureResult = await _queryExecutor.ExecuteAsync(proc, GetImageAsyncResult);

            return procedureResult;
        }
        private byte[] GetImageAsyncResult(IDataReader oReader)
        {
            while (oReader.Read())
            {
                return SqlQueryResultParser.GetValue<byte[]>(oReader, "ImageData");
            }
            return new Byte[] { };
        }


    }
}