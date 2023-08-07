namespace tech_blog_backend.Services.AnnouncementService
{
    public interface IAnnouncementService
    {
        Task<ServiceResponse<List<GetAnnouncementDto>>> GetAllAnnouncements();
        Task<ServiceResponse<GetAnnouncementDto>> GetAnnouncementById(int id);
        Task<ServiceResponse<List<GetAnnouncementDto>>> AddAnnouncement(AddAnnouncementDto newAnnouncement);
        Task<ServiceResponse<GetAnnouncementDto>> UpdateAnnouncement(UpdateAnnouncementDto updatedAnnouncement);
        Task<ServiceResponse<List<GetAnnouncementDto>>> DeleteAnnouncement(int id);

    }
}